using System.Runtime.Serialization;

namespace BankMore.Infra.Kafka.Events.ContaCorrente
{
    [DataContract]
    public class NumeroContaEncontradoResponseEvent
    {
        [DataMember(Order = 1)]
        public Guid CorrelationId { get; set; }
        [DataMember(Order = 2)]
        public int NumeroConta { get; set; }
    }
}
