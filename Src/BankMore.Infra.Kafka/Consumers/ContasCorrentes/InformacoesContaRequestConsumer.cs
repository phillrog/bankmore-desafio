using BankMore.Application.Common.Querys;
using BankMore.Application.ContasCorrentes.Querys;
using BankMore.Domain.Common;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Kafka.Events;
using BankMore.Infra.Kafka.Events.ContaCorrente;
using BankMore.Infra.Kafka.Producers;
using KafkaFlow;
using MathNet.Numerics.Statistics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
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
            InformacoesContaCorrenteQuery registerCommand;

            if (!string.IsNullOrEmpty(message.Cpf))
            {
                registerCommand = new InformacoesContaCorrenteQuery(message.Cpf);
            }
            else
            {
                registerCommand = new InformacoesContaCorrenteQuery(message.Numero);
            }

            var result = await bus.SendCommand<InformacoesContaCorrenteQuery, Result<InformacoesContaCorrenteDto>>(registerCommand);
            _logger.LogInformation($"[Lógica] levou {sw.ElapsedMilliseconds}ms.");

            var responseEvent = new NumeroContaEncontradoResponseEvent();
            if (result.IsSuccess)
            {
                responseEvent.CorrelationId = message.CorrelationId;
                responseEvent.NumeroConta = result?.Data.Numero ?? 0;
                responseEvent.IsSuccess = true;
                responseEvent.IdContaCorrente = result?.Data.Id ?? Guid.Empty;
            }
            else
            {
                responseEvent.CorrelationId = message.CorrelationId;
                responseEvent.IsSuccess = false;
                responseEvent.ErrorMessage = string.Join(" ,", result.Erros);
                responseEvent.ErrorType = result.ErroTipo;
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
}