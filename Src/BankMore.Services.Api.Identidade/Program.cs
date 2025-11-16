using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using BankMore.Infra.CrossCutting.IoC;
using BankMore.Services.Api.Identidade.Configurations;
using BankMore.Services.Api.Identidade.Controllers.V1;
using BankMore.Services.Apis.StartupExtensions;
using KafkaFlow;
using MediatR;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);
var apiNome = "API Identidade";
// START: Variables
// END: Variables

// START:
builder.Services.AddKafkaSetup();
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


// END: Custom services

var app = builder.Build();

// Configure the HTTP request pipeline.

// START: Custom middlewares

if (app.Environment.IsDevelopment())
{
    // ----- Error Handling -----
    app.UseCustomizedErrorHandling();
}

app.UseRouting();

// ----- CORS -----
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// ----- Auth -----
app.UseCustomizedAuth();

// ----- Controller -----
app.MapControllers();


// ----- Swagger UI -----
app.UseCustomizedSwagger(builder.Environment, apiNome);
// END: Custom middlewares


var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();
app.Run();
