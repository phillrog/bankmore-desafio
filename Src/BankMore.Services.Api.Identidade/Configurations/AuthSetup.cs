using BankMore.Infra.Apis.Configurations;
using BankMore.Infra.CrossCutting.Identity.Data;
using BankMore.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BankMore.Services.Api.Identidade.Configurations;

public static class AuthSetup
{
    public static IServiceCollection AddCustomizedAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration.GetValue<string>("SecretKey");
        if (string.IsNullOrEmpty(secretKey))
        {
            return services;
        }

        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = false;
            options.User.AllowedUserNameCharacters = null;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            // 1. Configurar o CPF (UserName)
            // O CPF é case sensitive, mas o Identity por padrão é case insensitive para UserName.
            // Para CPF, é recomendado forçar CaseSensitivity, mas o padrão 'false' geralmente funciona.
            options.User.RequireUniqueEmail = false; // Desativa a obrigatoriedade de e-mail Ãºnico

            // 2. Configurar o Email (Opcional)
            options.SignIn.RequireConfirmedEmail = false; // Não exige confirmação de e-mail no login

            // 3. Requisitos de Senha (Manter se necessário)
            options.Password.RequireDigit = true;
        });

        var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

        services.Configure<JwtIssuerOptions>(options =>
        {
            options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
            options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
            options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        });

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
        });

        services.AddAuthorization(options =>
        {
            PolicySetup.AddCustomPolicies(options);
        });

        return services;
    }

    public static IApplicationBuilder UseCustomizedAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
