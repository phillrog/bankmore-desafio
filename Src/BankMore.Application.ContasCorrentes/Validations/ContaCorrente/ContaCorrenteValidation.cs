using BankMore.Application.ContasCorrentes.Commands;
using FluentValidation;

namespace BankMore.Application.ContasCorrentes.Validations;

public abstract class ContaCorrenteValidation<T> : AbstractValidator<T>
    where T : ContaCorrenteCommand
{

    protected void ValidarNome()
    {
        RuleFor(c => c.Nome)
            .NotEmpty().WithMessage("Por favor informe o nome completo")
            .Length(3, 100).WithMessage("O nome deve ter entre 3 e 100 caracteres");
    }

    protected void ValidarSenha()
    {
        RuleFor(c => c.Senha)
            .NotEmpty().WithMessage("Por favor informe a senha")
            .MinimumLength(6).WithMessage("A senha deve ter no mÃ­nimo 6 caracteres.");
    }

    protected void ValidarNovaSenha()
    {
        RuleFor(c => c.SenhaAnterior)
        .MinimumLength(6).WithMessage("A senha anterior deve ter no mÃ­nimo 6 caracteres.")
        .When(c => !string.IsNullOrEmpty(c.Senha));

        RuleFor(c => c.Senha)
            .NotEmpty().WithMessage("Por favor informe a senha")
            .MinimumLength(6).WithMessage("A senha deve ter no mÃ­nimo 6 caracteres.")
            .When(c => !string.IsNullOrEmpty(c.SenhaAnterior));
    }


    protected void ValidarCpf()
    {
        RuleFor(c => c.Cpf)
            .NotEmpty().WithMessage("Por favor informe o CPF")
            .IsValidCPF().WithMessage("NÃºmero de CPF é inválido.");
    }

    protected void ValidarId()
    {
        RuleFor(c => c.Id)
            .NotEqual(Guid.Empty);
    }

    protected void ValidarNumero()
    {
        RuleFor(c => c.Numero)
            .GreaterThan(0)
            .WithMessage("Por favor informe o nÃºmero da conta.");
    }

}
