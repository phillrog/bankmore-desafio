using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankMore.Infra.CrossCutting.Identity.Filters
{
    /// <summary>
    /// O IAuthorizationHandler lida com a lógica de autorização. 
    /// Utilizado no ContaCorrenteController
    /// </summary>
    public class MustBeOwnerOrMasterHandler : IAuthorizationHandler
    {
        private static readonly string Bearer = "bearer";
        private readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();
        private readonly IHttpContextAccessor _httpContextAccessor;
        // Chave do Claim que contém o identificador do usuário (Conta) no JWT.
        private const string MasterRoleName = "Master";
        private const string AdminRoleName = "Admin";


        public MustBeOwnerOrMasterHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            #region [ VALIDAÇÕES ]

            //  Obtém o Requisito MustBeOwnerOrMasterRequirement
            var ownerOrMasterRequirement = context.Requirements
                .OfType<MustBeOwnerOrMasterRequirement>()
                .FirstOrDefault();

            if (ownerOrMasterRequirement == null)
            {
                // Este handler só lida com MustBeOwnerOrMasterRequirement então SAI
                return Task.CompletedTask;
            }

            Console.WriteLine("--- [MustBeOwnerOrMasterHandler] INICIANDO VALIDAÇÕO IAuthorizationHandler ---");
            var user = context.User;
            var token = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString();

            if ((!user.IsAuthenticated() && !string.IsNullOrEmpty(token)) || !user.Claims.Any(d => d.Value.Equals(AdminRoleName)))
            {
                var jwt = _handler.ReadJwtToken(token.Substring(Bearer.Length).TrimStart());

                user = new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims, JwtBearerDefaults.AuthenticationScheme));
            }

            // BYPASS MASTER: Se a Role for Master, acesso concedido imediatamente.
            // O Master pode consultar a própria conta ou qualquer outra.
            if (user.IsInRole(MasterRoleName) || user.Claims.Any(d => d.Value.Equals(MasterRoleName)))
            {
                Console.WriteLine("--- [MustBeOwnerOrMasterHandler] SUCESSO: Usuário é Master (BYPASS). Permissão total.");
                context.Succeed(ownerOrMasterRequirement);
                return Task.CompletedTask;
            }

            //  VERIFICAÇÕO INICIAL DE ROLE: Se não for Master e nem Admin, falha.
            if (!user.IsInRole(AdminRoleName))
            {
                Console.WriteLine("--- [MustBeOwnerOrMasterHandler] FALHA: Usuário não é Master e nem Admin. Bloqueado.");
                context.Fail();
                return Task.CompletedTask;
            }

            // --- Lógica de Comparação do Recurso (Aplicável APENAS para Admin) ---

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                Console.WriteLine("--- [MustBeOwnerOrMasterHandler] ERRO: HttpContext é NULL. Bloqueado.");
                context.Fail();
                return Task.CompletedTask;
            }

            // Obtém o IDENTIFICADOR do RECURSO da QUERY STRING (Leitura Robusta)
            var queryParameterKey = ownerOrMasterRequirement.RouteParameterName;
            string requestedAccountValue = null;

            // Ajuda a verificar se a chave está chegando corretamente ---
            var queryKeys = string.Join(", ", httpContext.Request.Query.Keys);
            Console.WriteLine($"--- [MustBeOwnerOrMasterHandler] DIAGNÃSTICO QUERY: Chaves na URL: [{queryKeys}]");
            Console.WriteLine($"--- [MustBeOwnerOrMasterHandler] DIAGNÃSTICO QUERY: Chave Esperada: '{queryParameterKey}'");

            #endregion

            #region [ EXTRAI VALORES DA QUERY ]

            // Busca pelo nome da chave exato (case sensitive)
            if (httpContext.Request.Query.TryGetValue(queryParameterKey, out StringValues value))
            {
                requestedAccountValue = value.FirstOrDefault();
            }

            // Busca insensÃ­vel ao case, caso o valor ainda seja nulo.
            if (string.IsNullOrEmpty(requestedAccountValue))
            {
                var foundKey = httpContext.Request.Query.Keys.FirstOrDefault(k =>
                    k.Equals(queryParameterKey, StringComparison.OrdinalIgnoreCase));

                if (foundKey != null && httpContext.Request.Query.TryGetValue(foundKey, out StringValues insensitiveValue))
                {
                    requestedAccountValue = insensitiveValue.FirstOrDefault();
                    Console.WriteLine($"--- [MustBeOwnerOrMasterHandler] DIAGNÃSTICO QUERY: Chave encontrada por Case Insensitive: '{foundKey}'");
                }
            }

            #endregion

            #region [ DESTINA O REQUEST ]

            // Obtém o IDENTIFICADOR do USUÃRIO do JWT (CPF/Conta)
            var userIdentifier = user.FindFirstValue(ownerOrMasterRequirement.OwnerClaimType);

            Console.WriteLine($"--- [MustBeOwnerOrMasterHandler] Valor Lido da Query: {requestedAccountValue ?? "NULL/Ausente"}");
            Console.WriteLine($"--- [MustBeOwnerOrMasterHandler] Identificador Logado: {userIdentifier ?? "NULL"}");

            // USUÃRIO LOGADO NULO
            if (string.IsNullOrEmpty(userIdentifier))
            {
                Console.WriteLine("--- [MustBeOwnerOrMasterHandler] FALHA: Identificador Logado nulo. Token inválido/ausente da Claim. Bloqueado.");
                context.Fail();
                return Task.CompletedTask;
            }

            // PARÃMETRO NULO/AUSENTE PARA ADMIN (PERMITIDO)
            if (string.IsNullOrEmpty(requestedAccountValue))
            {
                // Se o Admin não informou 'numeroConta', o Controller usará a conta dele.
                // Isso é permitido, pois é a consulta da própria conta.
                Console.WriteLine("--- [MustBeOwnerOrMasterHandler] SUCESSO: Usuário Admin consultando a própria conta (parÃ¢metro nulo/ausente). Permissão concedida.");
                context.Succeed(ownerOrMasterRequirement);
                return Task.CompletedTask;
            }

            // O valor fornecido deve ser o do Admin (Admin tentando acessar uma conta especÃ­fica).
            if (requestedAccountValue.Equals(userIdentifier, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("--- [MustBeOwnerOrMasterHandler] SUCESSO: Identificador da Query IGUAL ao Identificador do Token. Permissão concedida.");
                context.Succeed(ownerOrMasterRequirement);
            }
            else
            {
                // Se o valor foi fornecido, mas não é o seu.
                Console.WriteLine($"--- [MustBeOwnerOrMasterHandler] FALHA DE FILTRO: {requestedAccountValue} != {userIdentifier}. Admin tentando acessar conta de terceiros. Bloqueado (403 ESPERADO).");
                context.Fail();
            }

            return Task.CompletedTask;

            #endregion
        }
    }
}