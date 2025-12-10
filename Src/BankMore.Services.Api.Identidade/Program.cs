using BankMore.Infra.CrossCutting.IoC;
using BankMore.Services.Api.Identidade;
using BankMore.Services.Api.Identidade.Configurations;
using BankMore.Services.Api.Identidade.Controllers.V1;
using BankMore.Services.Apis.StartupExtensions;
using KafkaFlow;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Versioning;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var apiNome = "API Identidade";
// START: Variables
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
builder.Services.Configure<Microsoft.AspNetCore.Builder.CookiePolicyOptions>(options =>
{
    // Se estiver em desenvolvimento HTTP, use SameSite=Lax
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
});
// END: Variables

// START:
builder.Services.AddRazorPages();

/// kafka 
builder.Services.AddKafkaSetup(builder.Configuration);
// ----- Database -----
builder.Services.AddDatabaseSetup(builder.Configuration, builder.Environment);

// ----- Auth -----
builder.Services.AddCustomizedAuth(builder.Configuration);

// ----- Http -----
builder.Services.AddCustomizedHttp(builder.Configuration);

// ----- AutoMapper -----
builder.Services.AddAutoMapperSetup();

// Adding MediatR for Domain Events and Notifications
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

// ----- Hash -----
builder.Services.AddCustomizedHash(builder.Configuration);



// .NET Native DI Abstraction
NativeInjectorBootStrapper.RegisterServices(builder.Services);
builder.Services.AddServicesSetup();


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
builder.Services.AddCustomizedSwagger(builder.Environment, apiNome, typeof(AccountController));

builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});
// END: Custom services

var app = builder.Build();
var pathBase = builder.Configuration.GetValue<string>("Base");
app.UsePathBase(pathBase);

SeedData.EnsureSeedData(app.Services);

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

app.UseStaticFiles();
app.UseRouting();

app.UseCookiePolicy();

// ----- CORS -----
app.UseCors(CorsPolicyName);

// ----- Auth -----
app.UseCustomizedAuth();

app.UseAuthentication();

app.UseAuthorization();

// ----- Controller -----
app.MapControllers();


// ----- Swagger UI -----
app.UseCustomizedSwagger(builder.Environment, apiNome, pathBase);
// END: Custom middlewares

app.MapRazorPages();

var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();
app.Run();
