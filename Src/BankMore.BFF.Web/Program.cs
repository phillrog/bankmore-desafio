using BankMore.BFF.Web;
using BankMore.BFF.Web.Controllers.V1;
using BankMore.Services.Apis.StartupExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Versioning;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var apiNome = "API BFF Web";
// START: Variables
// END: Variables


const string CorsPolicyName = "AllowFrontendOrigins";

var allowedOrigins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName,
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
        });
});

// START: Custom services

// --- Configuração de Cookies OIDC/Sessão para ambiente HTTP Local ---
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // Essencial para permitir que o navegador envie cookies de SameSite=Lax/Unspecified em HTTP
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    options.OnAppendCookie = cookieContext =>
    {
        // Se não for HTTPS, defina Secure=false para evitar que o navegador bloqueie o cookie.
        if (cookieContext.Context.Request.IsHttps == false)
        {
            cookieContext.CookieOptions.Secure = false;
        }
    };
});


// --- 2. Auth Setup ---
builder.Services.AddBff(options =>
{

}).AddServerSideSessions();

// local APIs
builder.Services.AddControllers();

// cookie options
builder.Services.AddAuthentication(options =>
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
builder.Services.AddSingleton<ImpersonationAccessTokenRetriever>();

// -----Http---- -
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();



builder.Services.AddHttpClient("ContasCorrentesAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler(sp =>
{

    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

    return new TokenInjectionHandler(httpContextAccessor);
});

builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        x.JsonSerializerOptions.Encoder =
            System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);

        x.JsonSerializerOptions.WriteIndented = true;
        x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version"));
});

// Add ApiExplorer to discover versions
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

// ----- Swagger UI -----
builder.Services.AddCustomizedSwagger(builder.Environment, apiNome, typeof(ContasCorrentesController));

// END: Custom services

var app = builder.Build();
var pathBase = builder.Configuration.GetValue<string>("Base");
app.UsePathBase(pathBase);

// Globalization culture
var supportedCultures = new[] { new CultureInfo("pt-BR") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configure the HTTP request pipeline.

// START: Custom middlewares

if (app.Environment.IsDevelopment())
{
    // ----- Error Handling -----
    app.UseCustomizedErrorHandling();
}

app.UseRouting();

// UseCookiePolicy deve vir antes da autenticação/bff
app.UseCookiePolicy();

// ----- CORS -----
app.UseCors(CorsPolicyName);

// ----- Auth -----
app.UseAuthentication();
app.UseAuthorization();

// Ordem correta: BFF após Auth/Auth
app.UseBff();

app.MapControllers();


// ----- Swagger UI -----
app.UseCustomizedSwagger(app.Environment, apiNome, pathBase);


app.MapGet("/api/v1/status", async (HttpContext ctx) =>
{
    if (ctx.User.Identity?.IsAuthenticated == true)
    {
        var userName = ctx.User.Identity.Name;
        var subjectId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        return Results.Ok(new
        {
            status = "Authenticated",
            username = userName,
            subject_id = subjectId
        });
    } else
    {
        await ctx.SignOutAsync("cookie");
        
        // Dispara o fluxo de OIDC (logout/login)
        await ctx.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
        return Results.Unauthorized();
    }
});


app.Run();