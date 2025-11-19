using BankMore.Domain.Core.Commands;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Transferencias.Dtos;
using MediatR;

namespace BankMore.Application.Transferencias.Commands
{
    public abstract class TransferenciaCommand : Command, IRequest<Result<TransferenciaDto>>
    {
        public Guid Id { get; private set; }
        public int NumeroContaCorrenteOrigem { get; private set; }
        public int NumneroContaCorrenteDestino { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public decimal Valor { get; private set; }

        protected TransferenciaCommand(int numeroContaCorrenteOrigem, int numneroContaCorrenteDestino, decimal valor)
        {
            Id = new Guid();
            DataMovimento = DateTime.UtcNow;
            Valor = valor;
            NumeroContaCorrenteOrigem = numeroContaCorrenteOrigem;
            NumneroContaCorrenteDestino = numneroContaCorrenteDestino;
        }
    }
}
