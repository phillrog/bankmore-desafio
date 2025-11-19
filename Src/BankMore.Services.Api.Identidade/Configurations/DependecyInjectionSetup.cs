using BankMore.Application.Common.Querys;
using BankMore.Domain.Common;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Data.Common.Repository;
using MediatR;

namespace BankMore.Services.Api.Identidade.Configurations;

public static class DependecyInjectionSetup
{
    public static void AddServicesSetup(this IServiceCollection services)
    {
        // Application Common - Querys
        services.AddScoped<IRequestHandler<InformacoesContaCorrenteQuery, Result<InformacoesContaCorrenteDto>>, InformacoesContaCorrenteQueryHandler>();

        // Infra - Data Common
        services.AddScoped<IInformacoesContaRespository, InformacoesContaRespository>();
    }
}
