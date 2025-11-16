using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BankMore.Services.Apis.StartupExtensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddCustomizedSwagger(this IServiceCollection services, IWebHostEnvironment env, string api, params Type[] assemblyAnchorTypes)
    {

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
                    c.IncludeXmlComments(xmlPath);
                }
            }

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Desafio BankMore - " + api,
                Description = string.Empty,
                Contact = new OpenApiContact { Name = "Phillipe Souza" },
            });

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

                        // new string[] { }
                    },
            });

        });

        return services;
    }

    public static IApplicationBuilder UseCustomizedSwagger(this IApplicationBuilder app, IWebHostEnvironment env, string api)
    {

        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
        // specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"Desafio BankMore {api} v1.0");

        });


        return app;
    }
}
