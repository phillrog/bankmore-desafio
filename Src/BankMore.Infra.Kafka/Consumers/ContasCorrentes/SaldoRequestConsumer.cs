using BankMore.Application.ContasCorrentes.Querys.ContaCorrente;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Core.Notifications;
using BankMore.Infra.Kafka.Events.ContaCorrente;
using BankMore.Infra.Kafka.Producers;
using KafkaFlow;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BankMore.Infra.Kafka.Consumers;

public class SaldoRequestConsumer : IMessageHandler<SaldoRequestEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMessageProducer<ISaldoResponseProducer> _responseProducer;
    private readonly ILogger<SaldoRequestConsumer> _logger;
    private readonly List<DomainNotification> _notifications = new List<DomainNotification>();

    public SaldoRequestConsumer(
        IServiceScopeFactory serviceScopeFactory,
        IMessageProducer<ISaldoResponseProducer> producer,
        ILogger<SaldoRequestConsumer> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _responseProducer = producer;
        _logger = logger;
    }

    public async Task Handle(IMessageContext context, SaldoRequestEvent message)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation($"[Kafka] Recebido em {sw.ElapsedMilliseconds}ms.");
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var bus = serviceProvider.GetRequiredService<IMediatorHandler>();

        var query = new SaldoQuery() { NumeroConta = message.NumeroConta };
        var result = await bus.SendCommand<SaldoQuery, Result<SaldoDto>>(query);
        _logger.LogInformation($"[Lógica] levou {sw.ElapsedMilliseconds}ms.");

        SaldoResponseEvent responseEvent;

        if (result.IsSuccess)
        {
            responseEvent = new SaldoResponseEvent
            {
                CorrelationId = message.CorrelationId,
                IsSuccess = true,
                NumeroConta = result.Data.NumeroConta ?? 0,
                Saldo = result.Data.SaldoAtualizado,
                TotalCredito = result.Data.TotalCredito,
                TotalDebito = result.Data.TotalDebito,
            };
        }
        else
        {
            responseEvent = new SaldoResponseEvent
            {
                CorrelationId = message.CorrelationId,
                IsSuccess = false,
                ErrorMessage = string.Join(" ,", result.Erros),
                ErrorType = result.ErroTipo
            };
        }

        await _responseProducer.ProduceAsync(
            message.ReplyTopic,
            message.CorrelationId.ToString(),
            responseEvent
        );

        sw.Stop();
        _logger.LogInformation($"[Total]  levou {sw.ElapsedMilliseconds}ms.");
    }

}
