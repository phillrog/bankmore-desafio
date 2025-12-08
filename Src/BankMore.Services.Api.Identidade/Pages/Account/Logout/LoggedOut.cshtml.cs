using BankMore.Services.Api.Identidade.Pages.Account.Logout;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankMore.Services.Api.Identidade.Pages.Logout
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class LoggedOut : PageModel
    {
        private readonly IIdentityServerInteractionService _interactionService;
        
        public LoggedOutViewModel View { get; set; }

        public LoggedOut(IIdentityServerInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        public async Task OnGet(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interactionService.GetLogoutContextAsync(logoutId);

            View = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = LogoutOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = String.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };
        }
    }
}