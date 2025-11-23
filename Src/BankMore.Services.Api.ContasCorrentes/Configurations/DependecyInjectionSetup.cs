using BankMore.Application.Common.Commands;
using BankMore.Application.Common.Querys;
using BankMore.Application.Common.Querys.ContaCorrente;
using BankMore.Application.ContasCorrentes.Commands;
using BankMore.Application.ContasCorrentes.Commands.Transferencia;
using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Application.ContasCorrentes.Querys;
using BankMore.Application.ContasCorrentes.Services;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Common;
using BankMore.Domain.Common.Dtos;
using BankMore.Domain.Common.Events;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.ContasCorrentes.Interfaces.Services;
using BankMore.Domain.ContasCorrentes.Services;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Data.Common.Repository;
using BankMore.Infra.Data.Common.Respository;
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
        services.AddScoped<IIdempotenciaService, IdempotenciaService>();
        services.AddScoped<IMovimentoService, MovimentoService>();

        // Application - Commands
        services.AddScoped<IRequestHandler<CadastrarNovaContaCorrenteCommand, Result<NumeroContaCorrenteDto>>, ContaCorrenteCommandHandler>();
        services.AddScoped<IRequestHandler<AlterarContaCorrenteCommand, bool>, ContaCorrenteCommandHandler>();

        // Application - Commands - Idempotencia
        services.AddScoped<IRequestHandler<CadastrarNovaIdempotenciaCommand, Result<bool>>, IdenmpotenciaCommandHandler>();

        // Application - Commands - Movimento
        services.AddScoped<IRequestHandler<CadastrarNovaMovimentacaoCommand, Result<MovimentacaoRelaizadaDto>>, MovimentoCommandHandler>();
        services.AddScoped<IRequestHandler<TransferenciaCommand, bool>, MovimentoCommandHandler>();

        // Application - Commands - Transferencia
        services.AddScoped<IRequestHandler<DebitarContaCommand, MovimentacaoContaRespostaEvent>, DebitarContaCommandHandler>();
        services.AddScoped<IRequestHandler<TentarCreditoCommand, MovimentacaoContaRespostaEvent>, TentarCreditarContaCommandHandler>();

        // Application Common - Querys
        services.AddScoped<IRequestHandler<InformacoesContaCorrenteQuery, Result<InformacoesContaCorrenteDto>>, InformacoesContaCorrenteQueryHandler>();


        // Application - Querys - Idempotencia
        services.AddScoped<IRequestHandler<IdempotenciaViewQuery, Result<IdempotenciaViewModel>>, IdempotenciaQueryHandler>();
        services.AddScoped<IRequestHandler<IdempotenciaExisteQuery, bool>, IdempotenciaQueryHandler>();

        // Application - Querys - Movimento
        services.AddScoped<IRequestHandler<MovimentoViewQuery, Result<MovimentoViewModel>>, MovimentoQueryHandler>();
        services.AddScoped<IRequestHandler<InformacoesContaCorrenteQuery, Result<InformacoesContaCorrenteDto>>, InformacoesContaCorrenteQueryHandler>();

        // Application - Querys - Saldo
        services.AddScoped<IRequestHandler<SaldoQuery, Result<SaldoDto>>, InformacoesQueryHandler>();
        
        // Domain - Services
        services.AddScoped<IGeradorNumeroService, GeradorNumeroService>();
        services.AddScoped<ICorrentistaService, CorrentistaService>();

        // Infra - Data
        services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
        services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();
        services.AddScoped<IMovimentoRepository, MovimentoRepository>();

        // Infra - Data Common
        services.AddScoped<IInformacoesContaRespository, InformacoesContaRespository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
    }
}
