using BankMore.Application.ContasCorrentes.AutoMapper;

namespace BankMore.Services.Api.ContasCorrentes.Configurations;

public static class AutoMapperSetup
{
    public static void AddAutoMapperSetup(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddAutoMapper(AutoMapperConfig.RegisterMappings());
    }
}
