using BankMore.Infra.Apis.Configurations;
using BankMore.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BankMore.Services.Apis.StartupExtensions;

public static class JwtExtension
{
    public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration.GetValue<string>("SecretKey");
        if (string.IsNullOrEmpty(secretKey))
        {
            return services;
        }

        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

        var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

            ValidateAudience = true,
            ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            RequireExpirationTime = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(configureOptions =>
        {
            configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
            configureOptions.TokenValidationParameters = tokenValidationParameters;
            configureOptions.SaveToken = true;
            configureOptions.Events = JwtEventsSetup.GetCustomEvents();
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
