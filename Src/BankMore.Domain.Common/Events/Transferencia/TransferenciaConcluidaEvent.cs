using BankMore.Domain.Core.Events;

namespace BankMore.Domain.Common.Events
{
    public class TransferenciaConcluidaEvent : Event
    {
        public Guid Id { get; set; }
        public Guid IdContaCorrenteOrigem { get; set; }
        public Guid IdContaCorrenteDestino { get; set; }
        public decimal Valor { get; set; }
        public int Status { get; set; }
    }
}
