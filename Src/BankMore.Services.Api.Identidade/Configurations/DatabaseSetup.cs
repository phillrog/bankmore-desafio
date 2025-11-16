using BankMore.Infra.CrossCutting.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Services.Api.Identidade.Configurations;

public static class DatabaseSetup
{
    public static IServiceCollection AddDatabaseSetup(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddDbContext<AuthDbContext>(options =>
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
