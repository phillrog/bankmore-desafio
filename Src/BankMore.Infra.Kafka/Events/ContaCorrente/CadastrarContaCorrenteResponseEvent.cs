using System.Runtime.Serialization;

namespace BankMore.Infra.Kafka.Events.ContaCorrente
{
    [DataContract]
    public class CadastrarContaCorrenteResponseEvent
    {
        [DataMember(Order = 1)]
        public Guid CorrelationId { get; set; }
        [DataMember(Order = 2)]
        public int NumeroConta { get; set; }

        [DataMember(Order = 3)]
        public bool IsSuccess { get; set; }

        [DataMember(Order = 4)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 5)]
        public string ErrorType { get; set; }
    }
}
