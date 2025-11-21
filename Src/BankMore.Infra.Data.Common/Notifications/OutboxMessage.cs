using BankMore.Domain.Common.Interfaces;
using MediatR;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankMore.Infra.Data.Common.Notifications
{
    public class OutboxMessage
    {// ID da mensagem
        public Guid Id { get; private set; }

        // Data/hora em que a transação de DB foi comitada
        public DateTime CreatedOn { get; private set; }

        // Tipo do evento (Ex: "TransferenciaIniciadaEvent")
        public string Type { get; private set; }

        // O payload do evento (JSON serializado)
        public string Payload { get; private set; }

        // Status para o Worker saber se já foi processado
        public bool IsProcessed { get; private set; }
        public Guid CorrelationId { get; set; }
        public OutboxMessage()
        {

        }
        public OutboxMessage(INotification eventData)
        {
            Id = Guid.NewGuid();
            CreatedOn = DateTime.UtcNow;
            Type = eventData.GetType().AssemblyQualifiedName;
            Payload = JsonSerializer.Serialize(eventData, eventData.GetType(), new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            IsProcessed = false;

            if (eventData is ICorrelatedEvent correlatedEvent)
            {
                CorrelationId = correlatedEvent.CorrelationId;
            }
            else
            {
                // Fallback, embora não deva acontecer se a hierarquia de eventos estiver correta.
                CorrelationId = Guid.Empty;
            }
        }
    }
}
