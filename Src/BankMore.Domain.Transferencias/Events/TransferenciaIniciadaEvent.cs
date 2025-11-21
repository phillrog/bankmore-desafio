using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Core.Events;
using System.Runtime.Serialization;

namespace BankMore.Domain.Transferencias.Events
{
    /// Se colocar private nas propriedades set o serializer não grava os valores
    [DataContract]
    public class TransferenciaIniciadaEvent : Event, ICorrelatedEvent
    {
        public TransferenciaIniciadaEvent()
        {

        }
        public TransferenciaIniciadaEvent(Guid id, Guid correlationId, Guid idContaCorrenteOrigem, Guid idContaCorrenteDestino, decimal valor, int status, DateTime dataMovimento)
        {

            Id = id;
            IdContaCorrenteOrigem = idContaCorrenteOrigem;
            IdContaCorrenteDestino = idContaCorrenteDestino;
            Valor = valor;
            Status = status;
            CorrelationId = correlationId;
            DataMovimento = dataMovimento;
        }

        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public Guid IdContaCorrenteOrigem { get; set; }

        [DataMember(Order = 3)]
        public Guid IdContaCorrenteDestino { get; set; }

        [DataMember(Order = 4)]
        public decimal Valor { get; set; }

        [DataMember(Order = 5)]
        public int Status { get; set; }

        [DataMember(Order = 6)]
        public Guid CorrelationId { get; set; }

        [DataMember(Order = 7)]
        public DateTime DataMovimento { get; set; }

        [DataMember(Order = 8)]
        public string Topico { get; set; }

    }
}
