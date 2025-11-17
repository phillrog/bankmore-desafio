using BankMore.Application.ContasCorrentes.Commands;

namespace BankMore.Application.ContasCorrentes.Validations;

public class CadastrarNovaContaCorrenteValidation : ContaCorrenteValidation<CadastrarNovaContaCorrenteCommand>
{
    public CadastrarNovaContaCorrenteValidation()
    {
        ValidarNome();
        ValidarCpf();
        ValidarSenha();
    }
}
