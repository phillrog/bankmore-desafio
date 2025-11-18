using System.Runtime.Serialization;

namespace BankMore.Infra.Kafka.Events.Movimento
{
    [DataContract]
    public class MovimentacaoResponseEvent
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
        public Guid Id { get; set; }

        [DataMember(Order = 6)]
        public int NumeroConta { get; set; }

        [DataMember(Order = 7)]
        public string Nome { get; set; }
        [DataMember(Order = 8)]
        public DateTime DataHora { get; set; }
        [DataMember(Order = 9)]
        public char Tipo { get; set; }
        [DataMember(Order = 10)]
        public decimal Valor { get; set; }
        [DataMember(Order = 11)]
        public decimal SaldoAposMovimentacao { get; set; }
        [DataMember(Order = 12)]
        public string ReplyTopic { get; set; }
    }
}
