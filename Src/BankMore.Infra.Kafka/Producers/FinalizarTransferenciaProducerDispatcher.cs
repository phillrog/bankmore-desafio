using BankMore.Application.Transferencias.Commands;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Infra.Kafka.Tags;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;


namespace BankMore.Infra.Kafka.Producers
{
    public class FinalizarTransferenciaProducerDispatcher : IFinalizarTransferenciaProducerDispatcher
    {
        private readonly IServiceScopeFactory _serviceScopeFactor;
        public FinalizarTransferenciaProducerDispatcher(IServiceScopeFactory serviceScopeFactor)
        {
            _serviceScopeFactor = serviceScopeFactor;
        }

        public async Task PublishAsync(FinalizarTransferenciaCommand message, string topic)
        {
            using (var scope = _serviceScopeFactor.CreateScope())
            {
                var dispatcher = scope.ServiceProvider.GetService<IMessageProducer<IFinalizarTransferenciaProducerTag>>();

                await dispatcher.ProduceAsync(topic, message.CorrelationId.ToString(), message);
            }
        }
    }
}