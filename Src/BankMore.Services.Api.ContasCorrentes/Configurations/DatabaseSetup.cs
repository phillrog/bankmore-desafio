using BankMore.Infra.Data.ContasCorrentes.Context;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Services.Api.ContasCorrentes.Configurations;

public static class DatabaseSetup
{
    public static IServiceCollection AddDatabaseSetup(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var conn = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(conn);

            if (!env.IsProduction())
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}
