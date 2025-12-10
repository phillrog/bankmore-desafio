using BankMore.Domain.Common.Providers.Hash;
using BankMore.Domain.Core.Bus;
using BankMore.Infra.CrossCutting.Identity.Models;
using BankMore.Infra.CrossCutting.Identity.Models.AccountViewModels;
using BankMore.Infra.CrossCutting.Identity.Services;
using BankMore.Infra.Kafka.Services;
using BankMore.Services.Api.Identidade.Pages.Account.Login;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BankMore.Services.Api.Identidade.Pages.Login
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        // Removido: private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IIdentityProviderStore _identityProviderStore;

        // 🆕 Novas Dependências do ASP.NET Identity e Serviços Customizados
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMediatorHandler _mediator;
        private readonly InformacoesContaService _informacoesContaService;

        public ViewModel View { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public Index(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IIdentityProviderStore identityProviderStore,
            IEventService events,
            // 🆕 Injeções Customizadas
            UserManager<ApplicationUser> userManager,
            IPasswordHasher passwordHasher,
            IMediatorHandler mediator,
            InformacoesContaService informacoesContaService)
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _identityProviderStore = identityProviderStore;
            _events = events;

            // 🆕 Inicialização das dependências customizadas
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _mediator = mediator;
            _informacoesContaService = informacoesContaService;
        }

        // --- OnGet (Permanece o mesmo) ---
        public async Task<IActionResult> OnGet(string returnUrl)
        {
            await BuildModelAsync(returnUrl);

            // 💡 PASSO 1: VERIFICAÇÃO DE SSO (Sessão Válida)

            // Verifica se o usuário JÁ está autenticado localmente pelo Duende/Identity
            if (User.Identity.IsAuthenticated)
            {
                // 1.1 Se o returnUrl estiver VAZIO, significa que o usuário digitou
                // o link do login diretamente no browser (localhost:5000/account/login)
                if (string.IsNullOrEmpty(returnUrl))
                {
                    // 1.2 Redireciona para o cliente Angular (Fallback)
                    // Use a sua URI de redirect configurada para o Angular (localhost:4200)

                    var client = await _clientStore.FindClientByIdAsync("identity");
                    var angularHomeUri = client?.PostLogoutRedirectUris.FirstOrDefault();

                    if (!string.IsNullOrEmpty(angularHomeUri))
                    {
                        // 🚀 Redireciona para o Angular
                        return Redirect(angularHomeUri);
                    }

                    // Faz login
                    return Redirect("~/");
                }

                // Se o usuário estiver autenticado, mas houver um returnUrl, 
                // significa que ele faz parte de um fluxo OIDC (e deve ser processado)
                // O Duende/IdentityServer fará a validação da sessão OIDC (Grant/Consent)

                // Nota: A linha abaixo garante que usuários autenticados localmente não vejam
                // a tela de login se eles já tiverem uma sessão OIDC ativa.
                var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
                if (context == null)
                {
                    // É um redirecionamento OIDC (returnUrl) que não precisa de tela de login
                    // Retorna para o fluxo OIDC (ex: consentimento)
                    return Redirect(returnUrl);
                }
            }

            // 💡 PASSO 2: CONTINUA O FLUXO DE LOGIN NORMAL (Se não estiver autenticado)

            if (View.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToPage("/ExternalLogin/Challenge/Index", new { scheme = View.ExternalLoginScheme, returnUrl });
            }

            return Page(); // Mostra a tela de login
        }

        // --- OnPost (Com a Lógica de Validação Customizada) ---
        public async Task<IActionResult> OnPost()
        {
            var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

            // o usuário clicou no botão "cancelar"
            if (Input.Button != "login")
            {
                if (context != null)
                {
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    if (context.IsNativeClient())
                    {
                        return this.LoadingPage(Input.ReturnUrl);
                    }

                    return Redirect(Input.ReturnUrl);
                }

                return Redirect("~/");
            }

            if (ModelState.IsValid)
            {
                // 1. Encontrar usuário pelo Cpf (que é o UserName)
                var appUser = await _userManager.FindByNameAsync(Input.Username);

                bool isPasswordValid = false;

                if (appUser != null)
                {
                    // 2. Aplicar sua lógica de Salt (Password + Id do usuário)
                    var senhaComSalt = Input.Password + appUser.Id.ToLower();
                    var (isValid, _) = _passwordHasher.Check(appUser.PasswordHash, senhaComSalt);
                    isPasswordValid = isValid;
                }

                if (isPasswordValid)
                {
                    // LOGIN BEM SUCEDIDO

                    // Dispara evento de sucesso (opcional, Duende já faz isso)
                    await _events.RaiseAsync(new UserLoginSuccessEvent(appUser.UserName, appUser.Id, appUser.UserName, clientId: context?.Client.ClientId));

                    // 3. Configurar propriedades de persistência (Remember Me)
                    AuthenticationProperties props = null;
                    if (LoginOptions.AllowRememberLogin && Input.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(LoginOptions.RememberMeLoginDuration)
                        };
                    } else
                    {
                        props = new AuthenticationProperties
                        {
                            ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(80)
                        };
                    }

                    // 4. Emitir o Cookie de Autenticação
                    // O SubjectId é o Id do ApplicationUser (string)
                    var isuser = new IdentityServerUser(appUser.Id)
                    {
                        DisplayName = appUser.UserName,
                        // Para claims customizadas (Roles, numero_conta), é obrigatório o IProfileService.
                        // O Duende chamará seu ProfileService após este SignInAsync para enriquecer a sessão e tokens.
                    };

                    await HttpContext.SignInAsync(isuser, props); // EMITE O COOKIE!

                    if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            return this.LoadingPage(Input.ReturnUrl);
                        }
                        return Redirect(Input.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(Input.ReturnUrl))
                    {
                        return Redirect(Input.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(Input.ReturnUrl))
                    {
                        // Fallback: Redireciona para a primeira URI de Post-Logout configurada ou Home
                        var client = await _clientStore.FindClientByIdAsync("identity");
                        var redirectUri = client?.PostLogoutRedirectUris.FirstOrDefault() ?? "~/";

                        return Redirect(redirectUri);
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                // 🛑 CREDENCIAIS INVÁLIDAS (Usuário não encontrado ou Senha incorreta)
                await _events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials", clientId: context?.Client.ClientId));
                ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
            }

            // algo deu errado, mostre o formulário com erro
            await BuildModelAsync(Input.ReturnUrl);
            return Page();
        }

        // --- BuildModelAsync (Permanece o mesmo) ---
        private async Task BuildModelAsync(string returnUrl)
        {
            Input = new InputModel
            {
                ReturnUrl = returnUrl
            };

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                View = new ViewModel
                {
                    EnableLocalLogin = local,
                };

                Input.Username = context?.LoginHint;

                if (!local)
                {
                    View.ExternalProviders = new[] { new ViewModel.ExternalProvider { AuthenticationScheme = context.IdP } };
                }
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ViewModel.ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var dyanmicSchemes = (await _identityProviderStore.GetAllSchemeNamesAsync())
                .Where(x => x.Enabled)
                .Select(x => new ViewModel.ExternalProvider
                {
                    AuthenticationScheme = x.Scheme,
                    DisplayName = x.DisplayName
                });
            providers.AddRange(dyanmicSchemes);


            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            View = new ViewModel
            {
                AllowRememberLogin = LoginOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
                ExternalProviders = providers.ToArray()
            };
        }
    }
}