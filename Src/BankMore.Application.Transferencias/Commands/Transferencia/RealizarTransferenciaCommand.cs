using BankMore.Application.Transferencias.Validations;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Transferencias.Dtos;
using MediatR;

namespace BankMore.Application.Transferencias.Commands
{
    public class RealizarTransferenciaCommand : TransferenciaCommand, IRequest<Result<TransferenciaDto>>
    {
        public RealizarTransferenciaCommand(int numeroContaCorrenteOrigem, int numneroContaCorrenteDestino, decimal valor)
        {
            Id = Guid.NewGuid();
            DataMovimento = DateTime.UtcNow;
            Valor = valor;
            NumeroContaCorrenteOrigem = numeroContaCorrenteOrigem;
            NumneroContaCorrenteDestino = numneroContaCorrenteDestino;
        }

        public override bool IsValid()
        {
            ValidationResult = new RealizarTransferenciaValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
