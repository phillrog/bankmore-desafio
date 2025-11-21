using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Common.Providers.Hash;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Notifications;
using BankMore.Infra.Apis.Configurations;
using BankMore.Infra.CrossCutting.Identity.Data;
using BankMore.Infra.CrossCutting.Identity.Models;
using BankMore.Infra.CrossCutting.Identity.Models.AccountViewModels;
using BankMore.Infra.CrossCutting.Identity.Services;
using BankMore.Infra.Kafka.Events;
using BankMore.Infra.Kafka.Services;
using BankMore.Services.Apis.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankMore.Services.Api.Identidade.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
public class AccountController : ApiController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AuthDbContext _dbContext;
    private readonly IUser _user;
    private readonly IJwtFactory _jwtFactory;
    private readonly ILogger _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly InformacoesContaService _informacoesContaService;
    private readonly ContaCorrenteService _contaCorrenteService;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        AuthDbContext dbContext,
        IUser user,
        IJwtFactory jwtFactory,
        ILoggerFactory loggerFactory,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler mediator,
        IPasswordHasher passwordHasher,
        InformacoesContaService informacoesContaService,
        ContaCorrenteService contaCorrenteService
        )
        : base(notifications, mediator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _user = user;
        _jwtFactory = jwtFactory;
        _logger = loggerFactory.CreateLogger<AccountController>();
        _passwordHasher = passwordHasher;
        _informacoesContaService = informacoesContaService;
        _contaCorrenteService = contaCorrenteService;
    }

    /// <summary>
    /// Autentica um usuário e gera um token JWT e um Refresh Token.
    /// </summary>
    /// <remarks>
    /// O login é feito verificando o hash da senha concatenada com o ID do usuário (CPF + ID).
    /// </remarks>
    /// <param name="model">Dados de login (CPF e Senha).</param>
    /// <returns>Retorna os tokens de acesso e refresh em caso de sucesso.</returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    [ProducesResponseType(typeof(TokenViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            NotifyModelStateErrors();
            return Response();
        }

        var appUser = await _userManager.FindByNameAsync(model.Cpf);

        if (appUser is null)
        {
            NotifyError("Falha", "Usuário ou senha inválidos.");
            return Response();
        }

        var senhaComSalt = model.Password + appUser.Id.ToLower();
        var (isPasswordValid, _) = _passwordHasher.Check(appUser.PasswordHash, senhaComSalt);

        // Sign In
        if (!isPasswordValid)
        {
            NotifyError("Falha", "Usuário ou senha inválidos.");
            return Response();
        }

        _logger.LogInformation(1, "Usuário logado.");
        return Response(await GenerateToken(appUser));
    }

    /// <summary>
    /// Registra um novo usuário no sistema de identidade e envia um evento para o Kafka.
    /// </summary>
    /// <remarks>
    /// O usuário é criado com a Role 'Admin' e Claims padrão. O hash da senha é gerado usando a senha concatenada com o ID do usuário.
    /// </remarks>
    /// <param name="model">Dados para o registro do novo usuário.</param>
    /// <returns>Retorna um status de sucesso ou a lista de erros de validação/Identity.</returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK)] // Retorna Response(null)
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            NotifyModelStateErrors();
            return Response();
        }

        // Add User
        var appUser = new ApplicationUser { Cpf = model.Cpf, UserName = model.Cpf, NormalizedUserName = model.Nome };

        var identityResult = await _userManager.CreateAsync(appUser);
        if (!identityResult.Succeeded)
        {
            AddIdentityErrors(identityResult);
            return Response();
        }

        var id = appUser.Id.ToLower();
        var password = model.Password + id;
        var (hash, salt) = _passwordHasher.Hash(password);

        var evento = new UsuarioCriadoEvent()
        {
            Cpf = model.Cpf,
            Senha = hash,
            Id = Guid.Parse(id),
            Nome = model.Nome
        };

        /// ---- Envia para o topico no kafka ----
        var result = await _contaCorrenteService.CadastrarConta(evento);

        if (!result.IsSuccess)
        {
            await _userManager.DeleteAsync(appUser);
            return ResponseResult(result);
        }

        // Add UserRoles
        identityResult = await _userManager.AddToRoleAsync(appUser, "Admin");
        if (!identityResult.Succeeded)
        {
            AddIdentityErrors(identityResult);
            return Response();
        }

        // Add UserClaims
        var userClaims = new List<Claim>
        {
            new Claim("Admin_Write", "Write"),
            new Claim("Admin_Remove", "Remove"),
            new Claim("Admin_Read", "Read"),
        };
        await _userManager.AddClaimsAsync(appUser, userClaims);

        if (identityResult.Succeeded)
        {
            // 2. Define o hash manualmente (Identity armazena o hash completo nesta coluna)
            appUser.PasswordHash = hash;
            await _userManager.UpdateAsync(appUser);
        }

        _logger.LogInformation(3, "User created a new account with password.");
        return ResponseResult(result);
    }

    /// <summary>
    /// Renova o Access Token usando um Refresh Token válido.
    /// </summary>
    /// <remarks>
    /// O Refresh Token atual é marcado como usado ('Used = true') e um novo par de tokens é gerado.
    /// </remarks>
    /// <param name="model">O Refresh Token a ser renovado.</param>
    /// <returns>Retorna um novo par de tokens de acesso e refresh.</returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("refresh")]
    [ProducesResponseType(typeof(TokenViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh(TokenViewModel model)
    {
        if (!ModelState.IsValid)
        {
            NotifyModelStateErrors();
            return Response();
        }

        // Get current RefreshToken
        var refreshTokenCurrent = _dbContext.RefreshTokens.SingleOrDefault(
            x => x.Token == model.RefreshToken && !x.Used && !x.Invalidated);
        if (refreshTokenCurrent is null)
        {
            NotifyError("RefreshToken", "Refresh token does not exist");
            return Response();
        }

        if (refreshTokenCurrent.ExpiryDate < DateTime.UtcNow)
        {
            // Update current RefreshToken
            refreshTokenCurrent.Invalidated = true;
            await _dbContext.SaveChangesAsync();
            NotifyError("RefreshToken", "Refresh token invalid");
            return Response();
        }

        // Get User
        var appUser = await _userManager.FindByIdAsync(refreshTokenCurrent.UserId);
        if (appUser is null)
        {
            NotifyError("User", "User does not exist");
            return Response();
        }

        // Remove current RefreshToken
        // _dbContext.Remove(refreshTokenCurrent);
        // await _dbContext.SaveChangesAsync();

        // Update current RefreshToken
        refreshTokenCurrent.Used = true;
        await _dbContext.SaveChangesAsync();

        return Response(await GenerateToken(appUser));
    }

    /// <summary>
    /// Retorna informações sobre o usuário autenticado na requisição atual.
    /// </summary>
    /// <remarks>
    /// Requer um token JWT válido no cabeçalho 'Authorization'.
    /// </remarks>
    /// <returns>Retorna o status de autenticação e a lista de claims (declarações) do usuário.</returns>
    [HttpGet]
    [Route("current")]
    [ProducesResponseType(StatusCodes.Status200OK)] // Retorna um objeto anónimo com IsAuthenticated e ClaimsIdentity
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrent()
    {
        return Response(new
        {
            IsAuthenticated = _user.IsAuthenticated(),
            ClaimsIdentity = _user.GetClaimsIdentity().Select(x => new { x.Type, x.Value }),
        });
    }

    /// <summary>
    /// Atribui uma nova Role a um usuário existente. Apenas para usuários MASTER.
    /// </summary>
    /// <remarks>
    /// Endpoint restrito para administradores MASTER.
    /// </remarks>
    /// <param name="cpf">O CPF (UserName) do usuário a ser modificado.</param>
    /// <param name="roleName">O nome da Role a ser atribuÃ­da (ex: 'Master').</param>
    /// <returns>Retorna um status de sucesso ou a lista de erros.</returns>
    [HttpPost]
    [Authorize(Policy = PolicySetup.MasterAccess)]
    [Route("update-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateRole([FromQuery] string cpf, [FromQuery] string roleName)
    {
        if (string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(roleName))
        {
            NotifyError("Entrada inválida", "CPF e nome da Role são obrigatórios.");
            return Response();
        }

        var appUser = await _userManager.FindByNameAsync(cpf);
        if (appUser is null)
        {
            NotifyError("Não encontrado", $"Usuário com CPF {cpf} não existe.");
            return Response();
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            NotifyError("Role inválida", $"A Role '{roleName}' não está registrada no sistema.");
            return Response();
        }

        var identityResult = await _userManager.AddToRoleAsync(appUser, roleName);

        if (identityResult.Succeeded)
        {
            _logger.LogInformation(4, $"Role '{roleName}' atribuÃ­da com sucesso ao usuário {cpf}.");
            return Response();
        }

        AddIdentityErrors(identityResult);
        return Response();
    }

    private async Task<TokenViewModel> GenerateToken(ApplicationUser appUser)
    {
        if (appUser is null || string.IsNullOrEmpty(appUser.UserName))
        {
            return new TokenViewModel();
        }

        var result = await _informacoesContaService.ObterNumeroContaPorCpf(appUser.UserName);

        // Init ClaimsIdentity
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, appUser.NormalizedUserName));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, appUser.Id));
        claimsIdentity.AddClaim(new Claim("numero_conta", result.Data.ToString()));

        // Get UserClaims
        var userClaims = await _userManager.GetClaimsAsync(appUser);
        claimsIdentity.AddClaims(userClaims);

        // Get UserRoles
        var userRoles = await _userManager.GetRolesAsync(appUser);
        claimsIdentity.AddClaims(userRoles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

        // ClaimsIdentity.DefaultRoleClaimType & ClaimTypes.Role is the same

        // Get RoleClaims
        foreach (var userRole in userRoles)
        {
            var role = await _roleManager.FindByNameAsync(userRole);
            if (role is not null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                claimsIdentity.AddClaims(roleClaims);
            }
        }

        // Generate access token
        var jwtToken = await _jwtFactory.GenerateJwtToken(claimsIdentity);

        // Add refresh token
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = appUser.Id,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMinutes(90),
            JwtId = jwtToken.JwtId,
        };
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return new TokenViewModel
        {
            AccessToken = jwtToken.AccessToken,
            RefreshToken = refreshToken.Token,
        };
    }
}
