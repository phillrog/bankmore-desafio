using BankMore.Application.Transferencias.AutoMapper;

namespace BankMore.Services.Api.Transferencias;

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
