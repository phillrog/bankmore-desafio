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
                    Scopes = { "contas_correntes_api" }
                },

                // Recurso 2: API Transferências
                new ApiResource("transferencias_api", "API Transferências")
                {
                    Scopes = { "transferencias_api" }
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
                        GrantType.ClientCredentials,
                        OidcConstants.GrantTypes.TokenExchange
                    },

                    RedirectUris = { "https://localhost:5000/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:5000/signin-oidc",
                    PostLogoutRedirectUris = { 
                        "https://localhost:5000/signout-callback-oidc",
                        "http://localhost:4200"
                    },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "contas_correntes_api", "transferencias_api", "geral_api" },

                    AccessTokenLifetime = 75, // Force refresh
                    AllowedCorsOrigins = { "http://localhost:4200" }
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
