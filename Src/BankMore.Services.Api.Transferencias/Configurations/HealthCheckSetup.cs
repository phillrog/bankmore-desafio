using BankMore.Infra.Data.Transferencias;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Services.Api.Transferencias.Configurations;

public static class HealthCheckSetup
{
    public static IServiceCollection AddCustomizedHealthCheck(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        if (env.IsProduction() || env.IsStaging())
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddDbContextCheck<ApplicationDbContext>();
            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(15); // time in seconds between check
            }).AddInMemoryStorage();
        }

        return services;
    }

    public static void UseCustomizedHealthCheck(IEndpointRouteBuilder endpoints, IWebHostEnvironment env)
    {
        if (env.IsProduction() || env.IsStaging())
        {
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });

            endpoints.MapHealthChecksUI(setup =>
            {
                setup.UIPath = "/hc-ui";
                setup.ApiPath = "/hc-json";
            });
        }
    }
}
