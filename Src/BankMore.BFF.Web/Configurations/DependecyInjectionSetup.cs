using BankMore.BFF.Web;
using BankMore.BFF.Web.Services;

namespace BankMore.Services.Api.ContasCorrentes.Configurations;

public static class DependecyInjectionSetup
{
    public static void AddServicesSetup(this IServiceCollection services)
    {
        // -----Http---- -
        services.AddHttpContextAccessor();
        services.AddHttpClient();

        // -----  Token
        services.AddSingleton<ImpersonationAccessTokenRetriever>();

        // ---- Services-----

        services.AddScoped<IContaCorrenteService, ContaCorrenteService>();
        services.AddScoped<IMovimentacaoService, MovimentacaoService>();
    }
}
