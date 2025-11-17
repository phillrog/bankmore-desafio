using BankMore.Application.ContasCorrentes.Commands;
using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Application.ContasCorrentes.Querys;
using BankMore.Application.ContasCorrentes.Services;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.ContasCorrentes.Services;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Data.ContasCorrentes.Repository;
using BankMore.Infra.Data.ContasCorrentes.UoW;
using MediatR;

namespace BankMore.Services.Api.ContasCorrentes.Configurations;

public static class DependecyInjectionSetup
{
    public static void AddServicesSetup(this IServiceCollection services)
    {
        // Infra - Data
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Application
        services.AddScoped<IContaCorrenteService, ContaCorrenteService>();       

        // Application - Commands
        services.AddScoped<IRequestHandler<CadastrarNovaContaCorrenteCommand, Result<NumeroContaCorrenteDto>>, ContaCorrenteHandler>();
        services.AddScoped<IRequestHandler<AlterarContaCorrenteCommand, bool>, ContaCorrenteHandler>();

        // Application - Querys
        services.AddScoped<IRequestHandler<InformacoesQuery, Result<InformacoesViewModel>>, InformacoesQueryHandler>();

        // Domain - Services
        services.AddScoped<IGeradorNumeroService, GeradorNumeroService>();

        // Infra - Data
        services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
    }
}
