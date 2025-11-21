using BankMore.Domain.Core.Commands;

namespace BankMore.Application.Transferencias.Commands
{
    public abstract class TransferenciaCommand : Command
    {
        public Guid Id { get; protected set; }
        public int NumeroContaCorrenteOrigem { get; protected set; }
        public int NumneroContaCorrenteDestino { get; protected set; }
        public DateTime DataMovimento { get; protected set; }
        public decimal Valor { get; protected set; }
    }
}
