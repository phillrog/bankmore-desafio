using BankMore.Application.ContasCorrentes.Commands;
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
using System;
using System.Diagnostics;

namespace BankMore.Infra.Kafka.Consumers;

public class CadastrarContaRequestConsumer : IMessageHandler<CadastrarContaCorrenteRequestEvent>,
    INotificationHandler<DomainNotification>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMessageProducer<IInforcacoesContaResponseProducer> _responseProducer;
    private readonly ILogger<CadastrarContaRequestConsumer> _logger;
    private readonly List<DomainNotification> _notifications = new List<DomainNotification>();

    public CadastrarContaRequestConsumer(
        IServiceScopeFactory serviceScopeFactory,
        IMessageProducer<IInforcacoesContaResponseProducer> producer,
        ILogger<CadastrarContaRequestConsumer> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _responseProducer = producer;
        _logger = logger;
    }

    public async Task Handle(IMessageContext context, CadastrarContaCorrenteRequestEvent message)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation($"[Kafka] Recebido em {sw.ElapsedMilliseconds}ms.");
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var bus = serviceProvider.GetRequiredService<IMediatorHandler>();

        var registerCommand = new CadastrarNovaContaCorrenteCommand(message.Id, message.Nome, message.Senha, message.Cpf);
        var result = await bus.SendCommand<CadastrarNovaContaCorrenteCommand, Result<NumeroContaCorrenteDto>>(registerCommand);
        _logger.LogInformation($"[Lógica] levou {sw.ElapsedMilliseconds}ms.");

        CadastrarContaCorrenteResponseEvent responseEvent;

        if (result.IsSuccess)
        {
            responseEvent = new CadastrarContaCorrenteResponseEvent
            {
                CorrelationId = message.CorrelationId,
                IsSuccess = true,
                NumeroConta = result.Data.NumeroConta,
            };
        }
        else
        {
            responseEvent = new CadastrarContaCorrenteResponseEvent
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

    public Task Handle(DomainNotification notification, CancellationToken cancellationToken)
    {
        _notifications.Add(notification);
        return Task.CompletedTask;
    }
}
