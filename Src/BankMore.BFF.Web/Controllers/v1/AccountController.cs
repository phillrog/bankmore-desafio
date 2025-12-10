using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.BFF.Web.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login(string returnUrl = "/")
        {
            // Inicia o fluxo OIDC
            return Challenge(
                new AuthenticationProperties { RedirectUri = returnUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            await HttpContext.SignOutAsync("cookie");
            return SignOut(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
