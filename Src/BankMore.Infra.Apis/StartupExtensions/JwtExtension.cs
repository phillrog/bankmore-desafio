using BankMore.Infra.Apis.Configurations;
using BankMore.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BankMore.Services.Apis.StartupExtensions;

public static class JwtExtension
{
    public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var identityServerUrl = configuration.GetValue<string>("IdentityServerUrl");
        var apiResourceName = configuration.GetValue<string>("ApiResourceName");

        if (string.IsNullOrEmpty(identityServerUrl) || string.IsNullOrEmpty(apiResourceName))
        {
            return services;
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.BackchannelHttpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } };
            options.SaveToken = true;

            var httpClient = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler())
            {
                Timeout = options.BackchannelTimeout,
                MaxResponseContentBufferSize = 1024 * 1024 * 10 // 10 MB 
            };

            options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{identityServerUrl}/.well-known/openid-configuration/jwks",
                new JwksRetriever(),
                new HttpDocumentRetriever(httpClient) { RequireHttps = options.RequireHttpsMetadata });

            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidIssuer = identityServerUrl;
            options.TokenValidationParameters.ValidAudience = apiResourceName;

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    // Este log mostrará a exceção (ex: Invalid signature, expired token, issuer mismatch)
                    Console.WriteLine($"JWT Bearer Authentication FAILED: {context.Exception.Message}");
                    return Task.CompletedTask;
                }
            };
        });


        services.AddAuthorization(options =>
        {
            PolicySetup.AddCustomPolicies(options);
        });

        return services;
    }

    public static IApplicationBuilder UseJwtConfiguration(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
