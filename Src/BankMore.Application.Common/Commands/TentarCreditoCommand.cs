using BankMore.Domain.Common.Events;
using MediatR;
using System.Runtime.Serialization;

namespace BankMore.Application.Common.Commands
{
    [DataContract]
    public class TentarCreditoCommand : IRequest<MovimentacaoContaRespostaEvent>
    {
        public TentarCreditoCommand() { }

        [DataMember(Order = 1)]
        public Guid Id { get; set; }
        [DataMember(Order = 2)]
        public Guid IdContaCorrenteOrigem { get; set; }
        [DataMember(Order = 3)]
        public Guid IdContaCorrenteDestino { get; set; }
        [DataMember(Order = 4)]
        public DateTime DataMovimento { get; set; }
        [DataMember(Order = 5)]
        public decimal Valor { get; set; }
        [DataMember(Order = 6)]
        public Guid CorrelationId { get; set; }
        [DataMember(Order = 7)]
        public string Topico { get; set; }
        [DataMember(Order = 8)]
        public bool IsCompensation { get; set; } = false;
    }
}
