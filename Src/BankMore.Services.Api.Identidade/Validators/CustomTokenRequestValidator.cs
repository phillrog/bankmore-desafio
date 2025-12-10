using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using System.Security.Claims;

namespace BankMore.Services.Api.Identidade.Validators
{
    public class CustomTokenRequestValidator : ICustomTokenRequestValidator
    {
        // Este método é chamado sempre que o endpoint /connect/token é invocado
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            var request = context.Result.ValidatedRequest;

            // 1. VERIFICA SE O FLUXO É CLIENT CREDENTIALS (S2S)
            if (request.GrantType == GrantType.ClientCredentials)
            {
                
                if (request.Client.ClientId == "bff_client")
                {
                    request.ClientClaims.Add(new Claim("role", "Master"));
                }

                // Adicione outras lógicas ou log aqui, se necessário.
            }

            // Retorno obrigatório, pois não estamos interrompendo o fluxo, apenas injetando claims.
            return Task.CompletedTask;
        }
    }
}