using BankMore.Application.Common.Commands;
using BankMore.Application.Common.Topicos.Saga;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Transferencias.Events;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;


namespace BankMore.Infra.Kafka.Consumers.Saga.Transferencia
{
    public class TransferenciaIniciadaHandler : IMessageHandler<TransferenciaIniciadaEvent>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TransferenciaIniciadaHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Handle(IMessageContext context, TransferenciaIniciadaEvent message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

                if (!await outboxRepository.ExistTransfer(message.CorrelationId)) return;

                var dispatcher = scope.ServiceProvider.GetRequiredService<IDebitarContaProducerDispatcher>();

                var debitCommand = new DebitarContaCommand
                {
                    Id = message.Id,
                    IdContaCorrenteOrigem = message.IdContaCorrenteOrigem,
                    IdContaCorrenteDestino = message.IdContaCorrenteDestino,
                    CorrelationId = message.CorrelationId,
                    Valor = message.Valor,
                    Status = message.Status,
                    DataMovimento = message.DataMovimento,
                    Topico = SagaTopico.IniciarTranferencia,
                };

                await dispatcher.PublishAsync(debitCommand, SagaTopico.DebitarConta);
            }
        }
    }
}
