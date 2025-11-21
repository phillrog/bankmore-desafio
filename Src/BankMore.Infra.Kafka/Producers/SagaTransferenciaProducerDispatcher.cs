using BankMore.Application.Common.Topicos.Saga;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Infra.Kafka.Tags;
using KafkaFlow;


namespace BankMore.Infra.Kafka.Producers
{
    public class SagaTransferenciaProducerDispatcher : ISagaTransferenciaProducerDispatcher
    {
        private readonly IMessageProducer<ISagaProducerTag> _sagaNovaTransferenciaProducer;

        public SagaTransferenciaProducerDispatcher(
            IMessageProducer<ISagaProducerTag> sagaNovaTransferenciaProducer)
        {
            _sagaNovaTransferenciaProducer = sagaNovaTransferenciaProducer;
        }

        public async Task PublishAsync(object message, string topic)
        {
            string key = GetKeyFromMessage(message);

            if (topic.Equals(SagaTopico.IniciarTranferencia, StringComparison.OrdinalIgnoreCase))
            {
                await _sagaNovaTransferenciaProducer.ProduceAsync(
                    topic,
                    key,
                    message
                );
            }
            else
            {
                throw new ArgumentException($"Tópico desconhecido ou não configurado para publicação: {topic}");
            }
        }

        private string GetKeyFromMessage(object message)
        {
            if (message == null) return Guid.NewGuid().ToString();

            var messageType = message.GetType();

            var correlationIdProperty = messageType.GetProperty("CorrelationId");
            if (correlationIdProperty != null)
            {
                var correlationIdValue = correlationIdProperty.GetValue(message)?.ToString();
                if (!string.IsNullOrEmpty(correlationIdValue) && !Guid.Parse(correlationIdValue).ToString().Equals(Guid.Empty.ToString()))
                {
                    return correlationIdValue;
                }
            }

            // Tenta pegar Id
            var idProperty = messageType.GetProperty("Id");
            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(message)?.ToString();
                if (!string.IsNullOrEmpty(idValue))
                {
                    return idValue;
                }
            }

            return Guid.NewGuid().ToString();
        }
    }
}