using BankMore.Domain.Common.Events;
using MediatR;
using System.Runtime.Serialization;


namespace BankMore.Application.Common.Commands
{
    [DataContract]
    public class EstornarDebitoContaCommand : IRequest<MovimentacaoContaRespostaEvent>
    {
        public EstornarDebitoContaCommand()
        {

        }
        public EstornarDebitoContaCommand(Guid id, Guid correlationId, Guid idContaCorrenteOrigem, Guid idContaCorrenteDestino, decimal valor, int status, DateTime dataMovimento)
        {
            Id = id;
            IdContaCorrenteOrigem = idContaCorrenteOrigem;
            IdContaCorrenteDestino = idContaCorrenteDestino;
            Valor = valor;
            Status = status;
            CorrelationId = correlationId;
            DataMovimento = dataMovimento;
            IsCompensation = true;
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
        public bool IsCompensation { get; set; } = false;
        [DataMember(Order = 9)]
        public string Descricao { get; set; }
    }
}
