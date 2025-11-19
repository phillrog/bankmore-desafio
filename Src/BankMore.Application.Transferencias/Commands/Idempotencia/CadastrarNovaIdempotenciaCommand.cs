
using BankMore.Application.Transferencias.Validations;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.Transferencias.Commands;

public class CadastrarNovaIdempotenciaCommand : IdempotenciaCommand, IRequest<Result<bool>>
{
    public CadastrarNovaIdempotenciaCommand(Guid id, Guid idContaCorrente, string resultado, string requisicao)
    {
        Id = id;
        IdContaCorrente = idContaCorrente;
        Requisicao = requisicao;
        Resultado = resultado;
    }

    public override bool IsValid()
    {
        ValidationResult = new CadastrarNovaIdempotenciaValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}
