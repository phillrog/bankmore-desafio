using BankMore.Domain.Common.Providers.Hash;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Services.Apis.StartupExtensions;

public static class HashingExtension
{
    public static IServiceCollection AddCustomizedHash(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HashingOptions>(configuration.GetSection(HashingOptions.Hashing));
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
