using BankMore.Application.ContasCorrentes.Commands;

namespace BankMore.Application.ContasCorrentes.Validations;

public class AlterarContaCorrenteValidation : ContaCorrenteValidation<AlterarContaCorrenteCommand>
{
    public AlterarContaCorrenteValidation()
    {
        ValidarNumero();
        ValidarNome();
        ValidarCpf();
        ValidarNovaSenha();
    }
}
