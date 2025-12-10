using Duende.IdentityModel;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Services.Api.Identidade
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),                
            ];

        public static IEnumerable<ApiScope> ApiScopes =>
            [
                // Escopo para a API de Contas Correntes
                new ApiScope("contas_correntes_api", "API de Gestão de Contas Correntes"), 

                // Escopo para a API de Transferências
                new ApiScope("transferencias_api", "API de Execução de Transferências"),              
            ];

        public static IEnumerable<ApiResource> ApiResources =>
            [
                // Recurso 1: API Contas Correntes
                new ApiResource("contas_correntes_api", "API Contas Correntes")
                {
                    // Os clientes precisam solicitar este scope para acessar este Recurso.
                    Scopes = { "contas_correntes_api" },
                    UserClaims = { "role", "sub", "numero_conta" }
                },

                // Recurso 2: API Transferências
                new ApiResource("transferencias_api", "API Transferências")
                {
                    Scopes = { "transferencias_api" },
                    UserClaims = { "role", "sub" }
                },
        
                // Recurso 3: API Geral (se mantiver o escopo "geral_api")
                new ApiResource("geral_api", "API Geral")
                {
                    Scopes = { "geral_api" }
                }
            ];

        public static IEnumerable<Client> Clients =>
            [
                new Client
                {
                    ClientId = "identity",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes =
                    {
                        GrantType.AuthorizationCode,
                        GrantType.ResourceOwnerPassword
                    },

                    RedirectUris = { "http://localhost:5003/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:5000/signin-oidc",
                    PostLogoutRedirectUris = {
                        "http://localhost:5000/signout-callback-oidc",
                        "http://localhost:4200"
                    },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "offline_access", "contas_correntes_api", "transferencias_api", "geral_api" },

                    AccessTokenLifetime = 75, // Force refresh
                    AllowedCorsOrigins = { "http://localhost:4200" },

                    RequirePkce = true,
                    RequireClientSecret = true,
                },
                new Client
                {
                    ClientId = "bff_client",
                    ClientSecrets = { new Secret("secret_bff".Sha256()) },

                    AllowedGrantTypes =
                    {
                        GrantType.AuthorizationCode, // Para pegar o token do usuário vindo do Angular (4200)
                        GrantType.ClientCredentials, // Para chamadas S2S internas (Ex: BFF -> 5001 sem usuário)
                        OidcConstants.GrantTypes.TokenExchange, // Recomendado para delegação
                    },

                    RedirectUris = { "http://localhost:5003/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:5003/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse, // Allow reuse of refresh tokens
                    RefreshTokenExpiration = TokenExpiration.Absolute, // Use absolute lifetime
                    AbsoluteRefreshTokenLifetime = 2592000, // 30
                    AllowedScopes =
                    {
                        "openid",
                        "profile",
                        "offline_access",
                        "contas_correntes_api",
                        "transferencias_api",
                    },

                    AccessTokenLifetime = 75 ,// Force refresh
                    ClientClaimsPrefix = null,
                    RequirePkce = true,
                    RequireClientSecret = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    UpdateAccessTokenClaimsOnRefresh = true
                },
                new Client
                {
                    ClientId = "contacorrente_client",
                    ClientSecrets = { new Secret("secret_conta_corrente".Sha256()) },

                    AllowedGrantTypes =
                    {
                        GrantType.ClientCredentials,
                    },

                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        "openid",
                        "profile",
                        "offline_access",
                        "contas_correntes_api",
                        "transferencias_api",
                    },

                    AccessTokenLifetime = 75 ,// Force refresh
                    ClientClaimsPrefix = null,
                    RequirePkce = true,
                    RequireClientSecret = true,
                },

                new Client
                {
                    ClientId = "transferencia_client",
                    ClientSecrets = { new Secret("secret_transferencia".Sha256()) },

                    AllowedGrantTypes =
                    {
                        GrantType.ClientCredentials,
                    },

                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        "openid",
                        "profile",
                        "offline_access",
                        "contas_correntes_api",
                    },

                    AccessTokenLifetime = 75 ,// Force refresh
                    ClientClaimsPrefix = null,
                    RequirePkce = true,
                    RequireClientSecret = true,
                },
            ];
    }

    public static class SeedData
    {
        // Método que garantirá que as migrações sejam aplicadas e o DB populado.
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                // 1. Persistência de Configuração (Clientes, Scopes, Resources)
                scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>()
                    .Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                // Popula Identity Resources (OpenId, Profile)
                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                // Popula API Scopes
                if (!context.ApiScopes.Any())
                {
                    foreach (var scopeItem in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(scopeItem.ToEntity());
                    }
                    context.SaveChanges();
                }

                // Popula API Resources
                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.ApiResources)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                // Popula Clients (bff_angular, etc.)
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }


                // 2. Persistência Operacional (Tokens, Grants)
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>()
                    .Database.Migrate();

                // 3. Persistência de Usuários (ASP.NET Identity)
                // Assumindo que você já tem um método para migrar e popular seu AuthDbContext.
            }
        }
    }
}