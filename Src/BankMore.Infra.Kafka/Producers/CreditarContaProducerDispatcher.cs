using BankMore.Application.Common.Commands;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Infra.Kafka.Tags;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;


namespace BankMore.Infra.Kafka.Producers
{
    public class CreditarContaProducerDispatcher : ICreditarContaProducerDispatcher
    {
        private readonly IServiceScopeFactory _serviceScopeFactor;
        public CreditarContaProducerDispatcher(IServiceScopeFactory serviceScopeFactor)
        {
            _serviceScopeFactor = serviceScopeFactor;
        }

        public async Task PublishAsync(TentarCreditoCommand message, string topic)
        {
            using (var scope = _serviceScopeFactor.CreateScope())
            {
                var dispatcher = scope.ServiceProvider.GetService<IMessageProducer<ICreditarContaProducerTag>>();

                await dispatcher.ProduceAsync(topic, message.CorrelationId.ToString(), message);
            }
        }
    }
}