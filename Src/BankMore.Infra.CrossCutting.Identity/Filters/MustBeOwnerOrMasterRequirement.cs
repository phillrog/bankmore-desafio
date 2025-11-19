using Microsoft.AspNetCore.Authorization;

namespace BankMore.Infra.CrossCutting.Identity.Filters
{
    /// <summary>
    /// Define o requisito de autorização: o usuário deve ser o proprietário do recurso
    /// (comparando um claim do usuário com um parÃ¢metro da rota/query) ou ter a role 'Master'.
    /// </summary>
    public class MustBeOwnerOrMasterRequirement : IAuthorizationRequirement
    {
        // A chave do parÃ¢metro de rota/query que deve ser comparado (ex: "numeroConta").
        public string RouteParameterName { get; private set; }

        // O tipo (chave) do Claim no token que armazena o identificador do usuário (ex: "cpf", "idUsuario").
        public string OwnerClaimType { get; private set; }

        /// <summary>
        /// Inicializa o requisito.
        /// </summary>
        /// <param name="routeParameterName">Nome do parÃ¢metro na URL/Query (ex: "numeroConta").</param>
        /// <param name="ownerClaimType">Nome do claim no JWT para usar na validação (ex: "numeroConta").</param>
        public MustBeOwnerOrMasterRequirement(string routeParameterName, string ownerClaimType)
        {
            RouteParameterName = routeParameterName;
            OwnerClaimType = ownerClaimType;
        }
    }
}