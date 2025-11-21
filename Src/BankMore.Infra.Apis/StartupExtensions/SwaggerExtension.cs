using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Xml.XPath;

namespace BankMore.Services.Apis.StartupExtensions;

public static class SwaggerExtension
{
    /// <summary>
    /// Configura dinamicamente o SwaggerGen para criar um documento para cada versão de API.
    /// </summary>
    public static IServiceCollection AddCustomizedSwagger(this IServiceCollection services, IWebHostEnvironment env, string api, params Type[] assemblyAnchorTypes)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen(c =>
        {
            var assembliesToDocument = assemblyAnchorTypes
                .Select(t => t.Assembly)
                .Distinct()
                .ToList();

            // Adicione o assembly executando a extensão como fallback
            assembliesToDocument.Add(Assembly.GetExecutingAssembly());

            foreach (var assembly in assembliesToDocument.Distinct())
            {
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                // Verifica se o arquivo XML existe antes de tentar incluir
                if (File.Exists(xmlPath))
                {
                    // NOVO: Lê o arquivo XML explicitamente como UTF-8
                    using var fileStream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
                    c.IncludeXmlComments(() => new XPathDocument(streamReader), true);
                }
            }

            // Configuração de Segurança (Permanece aqui, pois é genérica)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                },
            });
        });

        return services;
    }

    /// <summary>
    /// Configura o Swagger UI para mostrar um endpoint para cada versão de API.
    /// </summary>
    public static IApplicationBuilder UseCustomizedSwagger(this IApplicationBuilder app, IWebHostEnvironment env, string api)
    {
        // Obtém o provedor de versão após o app.Build()
        var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
        app.UseSwaggerUI(c =>
        {
            // Cria dinamicamente um SwaggerEndpoint para CADA versão encontrada
            foreach (var description in provider.ApiVersionDescriptions)
            {
                c.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"Desafio BankMore {api} {description.GroupName.ToUpperInvariant()}");
            }

            // Opções de UI
            c.DefaultModelsExpandDepth(-1);
        });

        return app;
    }
}

/// <summary>
/// Classe auxiliar que configura dinamicamente um OpenApiInfo para cada versão.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    // O IApiVersionDescriptionProvider é injetado para descobrir as versões.
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // Adiciona um documento do Swagger para cada versão de API descoberta.
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                CreateVersionInfo(description));
        }
    }

    private static OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "BankMore API",
            Version = description.ApiVersion.ToString(),
            Description = "API para gestão de Contas Correntes.",
        };

        if (description.IsDeprecated)
        {
            info.Description += " **Esta versão da API está obsoleta.**";
        }

        return info;
    }
}