using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;

namespace BankMore.BFF.Web.Configurations;

public static class JwtSetup
{
    public static IServiceCollection AddJwtConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "cookie";
            // Usar a constante para alinhar com o AddOpenIdConnect
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie("cookie", options =>
        {
            // set session lifetime
            options.ExpireTimeSpan = TimeSpan.FromSeconds(5);

            // sliding or absolute
            options.SlidingExpiration = true;

            // host prefixed cookie name
            options.Cookie.Name = "bffcookie";

            // SameSite = Lax ou Unspecified para garantir que o cookie de sessão do BFF seja enviado após o redirecionamento OIDC
            options.Cookie.SameSite = SameSiteMode.Lax;

            // Necessário em ambiente HTTP (localhost)
            options.Cookie.SameSite = SameSiteMode.Lax; // OK para o redirecionamento OIDC
            options.Cookie.SecurePolicy = CookieSecurePolicy.None; // OK para HTTP
            options.Cookie.IsEssential = true;
        })
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => // Esquema registrado com o nome padrão
        {
            // Authority deve ser HTTP se o 5000 estiver em HTTP
            options.Authority = "http://localhost:5000";

            // confidential client using code flow + PKCE
            options.ClientId = "bff_client";
            options.ClientSecret = "secret_bff";
            options.ResponseType = "code";
            options.ResponseMode = "query";


            options.MapInboundClaims = false;
            options.ClaimActions.Add(new MapAllClaimsAction());
            options.ClaimActions.MapJsonKey("numero_conta", "numero_conta");
            options.GetClaimsFromUserInfoEndpoint = true;
            options.SaveTokens = true;
            options.DisableTelemetry = true;

            // request scopes + refresh tokens
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("contas_correntes_api");
            options.Scope.Add("transferencias_api");
            options.Scope.Add("offline_access");


            // para conexões HTTP backchannel
            options.RequireHttpsMetadata = false;

            // Garante que os cookies OIDC (Nonce e Correlação) usem a política para funcionar em HTTP
            options.NonceCookie.SameSite = SameSiteMode.Unspecified;
            options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;

            options.Events.OnTokenValidated = context =>
            {
                var numeroContaClaimValue = context.Principal?.FindFirst("numero_conta")?.Value;

                if (string.IsNullOrEmpty(numeroContaClaimValue))
                {
                    var identityToken = context.TokenEndpointResponse?.IdToken;
                    if (!string.IsNullOrEmpty(identityToken))
                    {

                        var rawClaim = context.Principal.FindFirst("numero_conta");
                        numeroContaClaimValue = rawClaim?.Value;
                    }
                }


                if (!string.IsNullOrEmpty(numeroContaClaimValue))
                {
                    // 2. Criar a nova claim e adicionar diretamente ao ClaimsPrincipal do usuário no BFF
                    var claimsIdentity = (ClaimsIdentity)context.Principal.Identity;

                    // A claim 'numero_conta' só deve ser adicionada se não existir, para evitar duplicação.
                    if (!claimsIdentity.HasClaim(c => c.Type == "numero_conta"))
                    {
                        claimsIdentity.AddClaim(new System.Security.Claims.Claim("numero_conta", numeroContaClaimValue));
                    }
                }

                return Task.CompletedTask;
            };
            options.Events.OnRemoteFailure = context =>
            {
                // Coloque um breakpoint aqui no debug
                Console.WriteLine($"Falha OIDC: {context.Failure.Message}");
                context.Response.Redirect("/erro?message=" + Uri.EscapeDataString(context.Failure.Message));
                context.HandleResponse();
                return Task.CompletedTask;
            };
        });
        return services;
    }

    public static IApplicationBuilder UseJwtConfig(this IApplicationBuilder app)
    {        
        app.UseAuthentication();
        app.UseAuthorization();

        
        return app;
    }
}