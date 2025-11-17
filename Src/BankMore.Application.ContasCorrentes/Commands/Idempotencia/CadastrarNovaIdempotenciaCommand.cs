
using BankMore.Application.ContasCorrentes.Validations;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Commands;

public class CadastrarNovaIdempotenciaCommand : IdempotenciaCommand, IRequest<Result<bool>>
{
    public CadastrarNovaIdempotenciaCommand(Guid idContaCorrente, string requisicao)
    {
        Id = Guid.NewGuid();
        IdContaCorrente = idContaCorrente;
        Requisicao = requisicao;
    }

    public override bool IsValid()
    {
        ValidationResult = new CadastrarNovaIdempotenciaValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}
