using BankMore.Application.ContasCorrentes.Commands;
using FluentValidation;

namespace BankMore.Application.ContasCorrentes.Validations;

public abstract class IdempotenciaValidation<T> : AbstractValidator<T>
    where T : IdempotenciaCommand
{
    protected void ValidarId()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("O identificador da idempotência é obrigatÃ³rio.")
            .Must(id => id != Guid.Empty).WithMessage("O identificador da idempotência não pode ser um GUID vazio.")
            .Must(id =>
            {
                if (Guid.TryParse(id.ToString(), out Guid result))
                {
                    return true;
                }
                return false;
            }).WithMessage("O identificador da idempotência informado não é um GUID válido.");
     
    }

    protected void ValidarRequisicao()
    {
        RuleFor(c => c.Requisicao)
            .NotEmpty().WithMessage("Por favor informe o json da requisição");
    }

    protected void ValidarResultado()
    {
        RuleFor(c => c.Resultado)
            .NotEmpty().WithMessage("Por favor informe o json do resultado");
    }
}
