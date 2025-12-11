using BankMore.Domain.Common.Events;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Commands.Transferencia
{
    public class RegistrarFalhaTransferenciaCommand : IRequest<MovimentacaoContaRespostaEvent>
    {
        public RegistrarFalhaTransferenciaCommand()
        {

        }
        public RegistrarFalhaTransferenciaCommand(Guid id, Guid correlationId, Guid idContaCorrenteOrigem, Guid idContaCorrenteDestino, decimal valor, int status, DateTime dataMovimento)
        {
            Id = id;
            IdContaCorrenteOrigem = idContaCorrenteOrigem;
            IdContaCorrenteDestino = idContaCorrenteDestino;
            Valor = valor;
            Status = status;
            CorrelationId = correlationId;
            DataMovimento = dataMovimento;
        }
        public Guid Id { get; set; }
        public Guid IdContaCorrenteOrigem { get; set; }
        public Guid IdContaCorrenteDestino { get; set; }
        public decimal Valor { get; set; }
        public int Status { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime DataMovimento { get; set; }
        public string Descricao { get; set; }
    }
}