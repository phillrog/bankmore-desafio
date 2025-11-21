using BankMore.Application.ContasCorrentes.Commands;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Core.Notifications;
using BankMore.Infra.Kafka.Events.Movimento;
using BankMore.Infra.Kafka.Tags;
using KafkaFlow;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BankMore.Infra.Kafka.Consumers;

public class MovimentacaoRequestConsumer : IMessageHandler<MovimentacaoRequestEvent>,
    INotificationHandler<DomainNotification>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<MovimentacaoRequestConsumer> _logger;
    private readonly List<DomainNotification> _notifications = new List<DomainNotification>();

    public MovimentacaoRequestConsumer(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MovimentacaoRequestConsumer> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task Handle(IMessageContext context, MovimentacaoRequestEvent message)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation($"[Kafka] Recebido em {sw.ElapsedMilliseconds}ms.");
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var bus = serviceProvider.GetRequiredService<IMediatorHandler>();

        var registerCommand = new CadastrarNovaMovimentacaoCommand(message.Valor, message.Tipo, message.Conta);
        var result = await bus.SendCommand<CadastrarNovaMovimentacaoCommand, Result<MovimentacaoRelaizadaDto>>(registerCommand);
        _logger.LogInformation($"[Lógica] levou {sw.ElapsedMilliseconds}ms.");

        MovimentacaoResponseEvent responseEvent;

        if (result.IsSuccess)
        {
            responseEvent = new MovimentacaoResponseEvent
            {
                CorrelationId = message.CorrelationId,
                IsSuccess = true,
                NumeroConta = result.Data.NumeroConta,
                Tipo = result.Data.Tipo,
                Valor = result.Data.Valor,
                DataHora = result.Data.DataHora,
                Nome = result.Data.Nome,
                Id = result.Data.Id,
                SaldoAposMovimentacao = result.Data.SaldoAposMovimentacao
            };
        }
        else
        {
            responseEvent = new MovimentacaoResponseEvent
            {
                CorrelationId = message.CorrelationId,
                IsSuccess = false,
                ErrorMessage = string.Join(" ,", result.Erros),
                ErrorType = result.ErroTipo
            };
        }

        var responseProducer = serviceProvider.GetRequiredService<IMessageProducer<IMovimentacaoResponseProducerTag>>();
        await responseProducer.ProduceAsync(
            message.ReplyTopic,
            message.CorrelationId.ToString(),
            responseEvent
        );

        sw.Stop();
        _logger.LogInformation($"[Total]  levou {sw.ElapsedMilliseconds}ms.");
    }

    public Task Handle(DomainNotification notification, CancellationToken cancellationToken)
    {
        _notifications.Add(notification);
        return Task.CompletedTask;
    }
}
