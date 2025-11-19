using BankMore.Application.Transferencias.Commands;
using FluentValidation;

namespace BankMore.Application.Transferencias.Validations
{
    public abstract class TransferenciaValidation<T> : AbstractValidator<T>
    where T : TransferenciaCommand
    {
        protected void ValidarId()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("O identificador da transferência é obrigatório.")
                .Must(id => id != Guid.Empty).WithMessage("O identificador da idempotência não pode ser um GUID vazio.")
                .Must(id =>
                {
                    if (Guid.TryParse(id.ToString(), out Guid result))
                    {
                        return true;
                    }
                    return false;
                }).WithMessage("O identificador da transferência informado não é um GUID válido.");
        }

        protected void ValidarDataMovimento()
        {
            RuleFor(c => c.DataMovimento)
                .GreaterThan(DateTime.MinValue)
                .WithMessage("Por favor informe uma data válida");
        }

        protected void ValidarValor()
        {
            RuleFor(c => c.Valor)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Por favor informe um valor positivo.");
        }

        protected void ValidarNumeroContaOrigem()
        {
            RuleFor(c => c.NumeroContaCorrenteOrigem)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o número da conta origem.");
        }

        protected void ValidarNumeroContaDestino()
        {
            RuleFor(c => c.NumeroContaCorrenteOrigem)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o número da conta destino.");
        }
    }
}