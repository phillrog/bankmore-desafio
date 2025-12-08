using BankMore.Infra.CrossCutting.Identity.Data;
using BankMore.Infra.CrossCutting.Identity.Models;
using BankMore.Services.Api.Identidade.Pages;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Necessário para UseSqlServer
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
            // Tratar erro ou usar um valor padrão seguro
            return services;
        }

        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

        // 1. CONFIGURAÇÃO COMPLETA DO ASP.NET IDENTITY
        // Registra ApplicationUser, IdentityRole, Entity Framework Stores, e todos os serviços de infraestrutura (incluindo IUserClaimsPrincipalFactory).
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Configurações do Identity (mantidas do seu código)
            options.User.RequireUniqueEmail = false;
            options.User.AllowedUserNameCharacters = null;
            options.Password.RequireDigit = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddRoles<IdentityRole>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();
        
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
            options.Cookie.IsEssential = true;
            options.LoginPath = "/Account/Login";
        }).AddDataProtection();

        // 2. CONFIGURAÇÃO DUENDE IDENTITY SERVER
        var isBuilder = services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.EmitStaticAudienceClaim = true;
            options.UserInteraction.LogoutUrl = "/Account/Logout";
            options.UserInteraction.LoginUrl = "/Account/Login";
        })
            .AddTestUsers(TestUsers.Users)
            .AddDeveloperSigningCredential() // credencial temporária
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsAssembly(typeof(AuthSetup).Assembly.FullName)
                );
            })
            // Persistência Operacional (Tokens, Grants)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsAssembly(typeof(AuthSetup).Assembly.FullName)
                );
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 3600;
            })
            .AddAspNetIdentity<ApplicationUser>();


        // 3. CONFIGURAÇÃO JWT OPTIONS (Para emissão de tokens)
        var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

        services.Configure<JwtIssuerOptions>(options =>
        {
            options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
            options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
            options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        });

        // 4. CONFIGURAÇÃO VALIDAÇÃO JWT (Para validar tokens nas APIs de Recurso)
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

        // 5. CONFIGURAÇÃO AUTHENTICATION SCHEMES (Dual: Cookie p/ BFF, JWT p/ APIs)
        services.AddAuthentication(options =>
        {
            // Padrão para login interativo (BFF)
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        }).AddJwtBearer(configureOptions =>
        {
            // JWT Bearer para APIs de Recurso
            configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
            configureOptions.TokenValidationParameters = tokenValidationParameters;
            configureOptions.SaveToken = true;
        });

        // 6. CONFIGURAÇÃO AUTHORIZATION (Com Policies de Scope)
        services.AddAuthorization(options =>
        {
            // Policies de Scope (Opcional, mas recomendado para proteger APIs)
            options.AddPolicy("ContasCorrentesScopePolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireClaim("scope", "contas_correntes_api");
            });

            options.AddPolicy("TransferenciasScopePolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireClaim("scope", "transferencias_api");
            });

            // Se você usa o PolicySetup, mantenha. Caso contrário, remova ou comente.
            // PolicySetup.AddCustomPolicies(options); 
        });

        return services;
    }

    public static IApplicationBuilder UseCustomizedAuth(this IApplicationBuilder app)
    {
        // 💡 O IdentityServer DEVE vir antes de UseAuthentication/UseAuthorization
        app.UseIdentityServer(); // Middleware do Duende IdentityServer        

        return app;
    }
}