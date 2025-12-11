using BankMore.Domain.Common.Providers.Hash;
using BankMore.Infra.CrossCutting.Identity.Models;
using BankMore.Infra.CrossCutting.Identity.Models.AccountViewModels;
using BankMore.Infra.Kafka.Events;
using BankMore.Infra.Kafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BankMore.Services.Api.Identidade.Pages.Register
{
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ContaCorrenteService _contaCorrenteService;
        private readonly ILogger<Index> _logger;


        // 1. O InputModel será o seu RegisterViewModel
        [BindProperty]
        public RegisterViewModel Input { get; set; }

        // Propriedade para exibir mensagem de sucesso/erro na View
        public string Message { get; set; }

        public Index(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IPasswordHasher passwordHasher,
            ContaCorrenteService contaCorrenteService,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _passwordHasher = passwordHasher;
            _contaCorrenteService = contaCorrenteService;
            _logger = loggerFactory.CreateLogger<Index>();
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/Account/Login/Index");
            }

            try
            {


                var model = Input;

                // Checa se o usuário já existe
                var existingUser = await _userManager.FindByNameAsync(model.Cpf);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "O CPF informado já está cadastrado.");
                    return Redirect("/Account/Login/Index");
                }

                // Add User
                var appUser = new ApplicationUser { Cpf = model.Cpf, UserName = model.Cpf, NormalizedUserName = model.Nome };

                var identityResult = await _userManager.CreateAsync(appUser);
                if (!identityResult.Succeeded)
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Redirect("/Account/Login/Index");
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
                    // Desfaz a criação do usuário se a comunicação com o Kafka falhar
                    await _userManager.DeleteAsync(appUser);
                    ModelState.AddModelError(string.Empty, $"Falha ao cadastrar conta: {result.Erros}");
                    TempData["ErrorMessage"] = $"Falha no registro: {string.Join(", ", result.Erros)}";
                    return Redirect("/Account/Login/Index");
                }

                identityResult = await _userManager.AddToRoleAsync(appUser, "Admin");

                if (!identityResult.Succeeded)
                {
                    TempData["ErrorMessage"] = $"Falha no registro: {string.Join(", ", result.Erros)}";
                }

                // Add UserClaims
                var userClaims = new List<Claim>
                {
                    new Claim("Admin_Write", "Write"),
                    new Claim("Admin_Remove", "Remove"),
                    new Claim("Admin_Read", "Read"),
                };
                await _userManager.AddClaimsAsync(appUser, userClaims);

                // 2. Define o hash e conta manualmente
                appUser.PasswordHash = hash;
                appUser.Conta = result.Data.NumeroConta.ToString();
                await _userManager.UpdateAsync(appUser);


                // SUCESSO!
                _logger.LogInformation(3, "User created a new account with password.");

                // Define a mensagem de sucesso e limpa o modelo para um novo registro
                Message = "Conta criada com sucesso! Faça login para continuar.";
                Input = new RegisterViewModel(); // Limpa os campos do formulário

                TempData["SuccessMessage"] = $"Conta número {result.Data.NumeroConta} criada com sucesso! Faça login para continuar.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Falha no registro: {ex.Message}";
            }

            return Redirect("/Account/Login/Index");
        }
    }
}
