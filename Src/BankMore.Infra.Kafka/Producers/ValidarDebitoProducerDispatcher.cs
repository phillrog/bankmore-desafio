using BankMore.Application.Transferencias.Interfaces;
using BankMore.Domain.Common.Events;
using BankMore.Infra.Kafka.Tags;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;


namespace BankMore.Infra.Kafka.Producers
{
    public class ValidarDebitoProducerDispatcher : IValidarDebitoProducerDispatcher
    {
        private readonly IServiceScopeFactory _serviceScopeFactor;
        public ValidarDebitoProducerDispatcher(IServiceScopeFactory serviceScopeFactor)
        {
            _serviceScopeFactor = serviceScopeFactor;
        }

        public async Task PublishAsync(MovimentacaoContaRespostaEvent message, string topic)
        {
            using (var scope = _serviceScopeFactor.CreateScope())
            {
                var dispatcher = scope.ServiceProvider.GetService<IMessageProducer<IValidarDebitoProducerTag>>();

                await dispatcher.ProduceAsync(topic, message.CorrelationId.ToString(), message);
            }
        }
    }
}