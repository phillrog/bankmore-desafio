using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace BankMore.Services.Api.Identidade.Pages.Logout
{
    internal class LogoutOptions
    {
        public static bool ShowLogoutPrompt = true;
        public static bool AutomaticRedirectAfterSignOut = false;
    }

    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;

        [BindProperty]
        public string LogoutId { get; set; }

        public Index(IIdentityServerInteractionService interaction, IEventService events)
        {
            _interaction = interaction;
            _events = events;
        }

        public async Task<IActionResult> OnGet(string logoutId)
        {
            LogoutId = logoutId;           
            return await OnPost();
        }

        public async Task<IActionResult> OnPost()
        {
            LogoutId ??= await _interaction.CreateLogoutContextAsync();

            if (User?.Identity.IsAuthenticated == true)
            {
                // Deleta o cookie de sessão do ASP.NET Identity
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

                // Deleta o cookie de sessão do Duende (o "idsrv.session")
                // Isso garante que o IdentityServer "saiba" que o usuário fez logout.
                await HttpContext.SignOutAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme);

                // Dispara o evento de sucesso de logout
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

                // Lógica de Logout Federado (Para provedores externos, se houver)
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    if (await HttpContext.GetSchemeSupportsSignOutAsync(idp))
                    {
                        // URL para onde o provedor externo deve redirecionar de volta
                        string url = Url.Page("/Account/Login/Index");

                        // Isso dispara um redirect para o provedor externo para ele limpar a sessão dele.
                        return SignOut(new AuthenticationProperties { RedirectUri = url }, idp);
                    }
                }
            }

            return RedirectToPage("/Account/Login/Index");
        }
    }
}