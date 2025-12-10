using Microsoft.AspNetCore.Authentication;

namespace BankMore.BFF.Web
{
    public class TokenInjectionHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenInjectionHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                // Busca o Access Token no contexto da sessão (gerado após o login OIDC)
                var accessToken = await httpContext.GetTokenAsync("access_token");

                if (!string.IsNullOrEmpty(accessToken))
                {
                    // Adiciona o token ao cabeçalho da requisição para a API Downstream
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                }
            }

            // Envia a requisição para a API Downstream
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
