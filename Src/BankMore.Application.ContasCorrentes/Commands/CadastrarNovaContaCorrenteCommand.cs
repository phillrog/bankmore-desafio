using BankMore.Application.ContasCorrentes.Validations;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Commands;

public class CadastrarNovaContaCorrenteCommand : ContaCorrenteCommand
{
    public CadastrarNovaContaCorrenteCommand(string nome, string senha, string cpf)
    {
        Nome = nome;
        Senha = senha;
        Cpf = cpf;
    }

    public CadastrarNovaContaCorrenteCommand(Guid id, string nome, string senha, string cpf)
    {
        Id = id;
        Nome = nome;
        Senha = senha;
        Cpf = cpf;
    }
    public override bool IsValid()
    {
        ValidationResult = new CadastrarNovaContaCorrenteValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}
