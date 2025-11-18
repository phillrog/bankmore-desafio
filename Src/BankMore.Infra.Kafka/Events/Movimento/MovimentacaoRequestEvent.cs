using BankMore.Domain.ContasCorrentes.Enums;
using System.Runtime.Serialization;

namespace BankMore.Infra.Kafka.Events.Movimento
{
    [DataContract]
    public class MovimentacaoRequestEvent
    {
        [DataMember(Order = 1)]
        public Guid CorrelationId { get; set; }
        [DataMember(Order = 2)]
        public bool IsSuccess { get; set; }

        [DataMember(Order = 3)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 4)]
        public string ErrorType { get; set; }

        [DataMember(Order = 5)]
        public TipoMovimento Tipo { get; set; }

        [DataMember(Order = 6)]
        public decimal Valor { get; set; }

        [DataMember(Order = 7)]
        public string ReplyTopic { get; set; }

        [DataMember(Order = 8)]
        public string Conta { get; set; }
    }
}
