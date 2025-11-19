using BankMore.Application.ContasCorrentes.Commands;
using FluentValidation;

namespace BankMore.Application.ContasCorrentes.Validations;

public abstract class MovimentoValidation<T> : AbstractValidator<T>
    where T : MovimentoCommand
{
    protected void ValidarId()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("O identificador da idempotÃªncia é obrigatÃ³rio.")
            .Must(id => id != Guid.Empty).WithMessage("O identificador da idempotÃªncia não pode ser um GUID vazio.")
            .Must(id =>
            {
                if (Guid.TryParse(id.ToString(), out Guid result))
                {
                    return true;
                }
                return false;
            }).WithMessage("O identificador da idempotÃªncia informado não é um GUID válido.");
     
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

    protected void ValidarTipoMovimento()
    {
        RuleFor(c => c.TipoMovimento)
            .NotEmpty()
            .WithMessage("O tipo de movimento é obrigatÃ³rio.")
            .Must(tipoMovimento =>
            {
                var nome = tipoMovimento.ToString();
                var valorChar = (char)tipoMovimento;
                var valoresAceitos = new List<string> { "Credito", "Debito", "C", "D" };
                return valoresAceitos.Contains(nome, StringComparer.OrdinalIgnoreCase) ||
                       valoresAceitos.Contains(valorChar.ToString(), StringComparer.OrdinalIgnoreCase);
            })
            .WithMessage("O tipo de movimento fornecido é inválido. Aceita-se 'C', 'D', 'Credito' ou 'Debito'.");
    }
}
