using BankMore.Application.Common.Commands;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Infra.Kafka.Tags;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;


namespace BankMore.Infra.Kafka.Producers
{
    public class EstornoDebitoContaProducerDispatch : IEstornoDebitoContaProducerDispatch
    {
        private readonly IServiceScopeFactory _serviceScopeFactor;
        public EstornoDebitoContaProducerDispatch(IServiceScopeFactory serviceScopeFactor)
        {
            _serviceScopeFactor = serviceScopeFactor;
        }

        public async Task PublishAsync(EstornarDebitoContaCommand message, string topic)
        {
            using (var scope = _serviceScopeFactor.CreateScope())
            {
                var dispatcher = scope.ServiceProvider.GetService<IMessageProducer<IEstornoDebitoContaTag>>();

                await dispatcher.ProduceAsync(topic, message.CorrelationId.ToString(), message);
            }
        }
    }
}