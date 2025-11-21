using BankMore.Application.Common.Querys;
using BankMore.Application.Transferencias.Commands;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Application.Transferencias.Querys;
using BankMore.Application.Transferencias.Services;
using BankMore.Application.Transferencias.ViewModels;
using BankMore.Domain.Common;
using BankMore.Domain.Common.Events;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Transferencias.Dtos;
using BankMore.Domain.Transferencias.Interfaces;
using BankMore.Infra.Data.Common.Repository;
using BankMore.Infra.Data.Transferencias.Repository;
using BankMore.Infra.Data.Transferencias.UoW;
using BankMore.Infra.Kafka.Services;
using MediatR;

namespace BankMore.Services.Api.Transferencias.Configurations;

public static class DependecyInjectionSetup
{
    public static void AddServicesSetup(this IServiceCollection services)
    {
        // Infra - Data
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Application
        services.AddScoped<ITransferenciaService, TransferenciaService>();
        services.AddScoped<IIdempotenciaService, IdempotenciaService>();

        // Application - Commands
        services.AddScoped<IRequestHandler<RealizarTransferenciaCommand, Result<TransferenciaDto>>, RealizarTransferenciaCommandHandler>();

        // Application Common - Querys
        services.AddScoped<IRequestHandler<InformacoesContaCorrenteQuery, Result<InformacoesContaCorrenteDto>>, InformacoesContaCorrenteQueryHandler>();

        // Application - Commands - Idempotencia
        services.AddScoped<IRequestHandler<CadastrarNovaIdempotenciaCommand, Result<bool>>, IdenmpotenciaCommandHandler>();

        // Application - Querys - Idempotencia
        services.AddScoped<IRequestHandler<IdempotenciaViewQuery, Result<IdempotenciaViewModel>>, IdempotenciaQueryHandler>();
        services.AddScoped<IRequestHandler<IdempotenciaExisteQuery, bool>, IdempotenciaQueryHandler>();

        // Domain - Services
        

        // Infra - Data
        services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
        services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

        // Infra - Data Common
        services.AddScoped<IInformacoesContaRespository, InformacoesContaRespository>();

        services.AddScoped<IRequestHandler<FinalizarTransferenciaCommand, MovimentacaoContaRespostaEvent>, FinalizarTransferenciaCommandHandler>();


    }
}
