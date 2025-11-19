using BankMore.Application.ContasCorrentes.Querys;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Kafka.Events;
using BankMore.Infra.Kafka.Events.ContaCorrente;
using BankMore.Infra.Kafka.Producers;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BankMore.Infra.Kafka.Consumers
{
    public class InformacoesContaRequestConsumer : IMessageHandler<BuscarNumeroContaEvent>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessageProducer<IInforcacoesContaResponseProducer> _responseProducer;
        private readonly ILogger<InformacoesContaRequestConsumer> _logger;

        public InformacoesContaRequestConsumer(
            IServiceScopeFactory serviceScopeFactory,
            IMessageProducer<IInforcacoesContaResponseProducer> producer,
            ILogger<InformacoesContaRequestConsumer> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _responseProducer = producer;
            _logger = logger;
        }

        public async Task Handle(IMessageContext context, BuscarNumeroContaEvent message)
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation($"[Kafka] Recebido em {sw.ElapsedMilliseconds}ms.");
            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            var bus = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
            var registerCommand = new InformacoesQuery(message.Cpf);

            var result = await bus.SendCommand<InformacoesQuery, Result<InformacoesViewModel>>(registerCommand);
            _logger.LogInformation($"[LÃ³gica] levou {sw.ElapsedMilliseconds}ms.");
            int numeroConta = result?.Data.Numero ?? 0;

            var responseEvent = new NumeroContaEncontradoResponseEvent
            {
                CorrelationId = message.CorrelationId,
                NumeroConta = numeroConta
            };

            await _responseProducer.ProduceAsync(
                message.ReplyTopic, 
                message.CorrelationId.ToString(), 
                responseEvent
            );

            sw.Stop();
            _logger.LogInformation($"[Total]  levou {sw.ElapsedMilliseconds}ms.");
        }
    }
}