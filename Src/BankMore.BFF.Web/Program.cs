using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using BankMore.BFF.Web;
using BankMore.BFF.Web.Configurations;
using BankMore.BFF.Web.Controllers.V1;
using BankMore.Services.Api.ContasCorrentes.Configurations;
using BankMore.Services.Apis.StartupExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Versioning;

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

builder.Services.AddCookieConfig();


// --- 2. Auth Setup ---
builder.Services.AddBff(options =>
{

}).AddServerSideSessions();

// local APIs
builder.Services.AddControllers();

// Jwt

builder.Services.AddJwtConfig(builder.Configuration);

// Services

builder.Services.AddServicesSetup();


// -----Clients----
builder.Services.AddClientsConfig();

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

// ----- CORS -----
app.UseCors(CorsPolicyName);


app.UseCookieConfig();


// ----- Auth -----
app.UseJwtConfig();

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