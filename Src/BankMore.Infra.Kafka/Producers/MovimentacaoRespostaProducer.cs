using BankMore.Domain.Common.Events;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Infra.Kafka.Producers
{
    public class MovimentacaoRespostaProducerTag { }

    public interface IMovimentacaoRespostaProducer
    {
        Task ProduceAsync(MovimentacaoContaRespostaEvent message, string key, string topic);
    }

    public class MovimentacaoRespostaProducer : IMovimentacaoRespostaProducer
    {
        private readonly IServiceProvider _serviceProvider;

        public MovimentacaoRespostaProducer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ProduceAsync(MovimentacaoContaRespostaEvent message, string key, string topic)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
        
                var kafkaProducer = scope.ServiceProvider
                    .GetRequiredService<IMessageProducer<MovimentacaoRespostaProducerTag>>();

                await kafkaProducer.ProduceAsync(topic, key, message);
            }
        }
    }
}
