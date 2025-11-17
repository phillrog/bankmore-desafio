using BankMore.Application.ContasCorrentes.Validations;

namespace BankMore.Application.ContasCorrentes.Commands;

public class AlterarContaCorrenteCommand : ContaCorrenteCommand
{
    public AlterarContaCorrenteCommand(string nome, string senha, string senhaAnterior, string cpf, bool ativo)
    {
        Nome = nome;
        Senha = senha;
        Cpf = cpf;
        SenhaAnterior = senhaAnterior;
        Ativo = ativo;
    }
    public override bool IsValid()
    {
        ValidationResult = new AlterarContaCorrenteValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}
