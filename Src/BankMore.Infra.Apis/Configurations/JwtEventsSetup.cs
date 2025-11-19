using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Infra.Apis.Configurations
{
    public static class JwtEventsSetup
    {
        /// <summary>
        /// Retorna um objeto JwtBearerEvents pré-configurado com manipuladores customizados.
        /// </summary>
        public static JwtBearerEvents GetCustomEvents()
        {
            return new JwtBearerEvents
            {
                // 1. OnChallenge (Captura o 401: Token Ausente/Inválido)
                OnChallenge = context =>
                {
                    context.HandleResponse(); // Impede o 401 padrão

                    var problemDetails = new ProblemDetails
                    {
                        Type = "https://bankmore.com/errors/authentication",
                        Title = "USER_UNAUTHORIZED",
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = "Credenciais de autenticação inválidas, ausentes ou expiradas. Token ausente ou inválido."
                    };

                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return context.Response.WriteAsJsonAsync(problemDetails);
                },

                // 2. OnForbidden (Captura o 403: Usuário Autenticado Falhou na Policy)
                OnForbidden = context =>
                {
                    // Este é o método que injeta a mensagem customizada no 403.
                    var problemDetails = new ProblemDetails
                    {
                        Type = "https://bankmore.com/errors/forbidden",
                        Title = "Acesso Negado (Forbidden)",
                        Status = StatusCodes.Status403Forbidden,
                        Detail = "Você não possui permissão para acessar o recurso solicitado.",
                    };

                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return context.Response.WriteAsJsonAsync(problemDetails);
                }
            };
        }
    }
}
