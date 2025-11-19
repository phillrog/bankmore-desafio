using BankMore.Application.Transferencias.Commands;
using BankMore.Application.Transferencias.Validations;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Transferencias.Dtos;
using MediatR;

namespace BankMore.Application.Transferencia.Commands.Transferencia
{
    internal class RealizarTransferenciaCommand : TransferenciaCommand, IRequest<Result<TransferenciaDto>>
    {
        public RealizarTransferenciaCommand(int numeroContaCorrenteOrigem, int numneroContaCorrenteDestino, decimal valor) :
            base(numeroContaCorrenteOrigem, numneroContaCorrenteDestino, valor)
        {
        }

        public override bool IsValid()
        {
            ValidationResult = new RealizarTransferenciaValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
