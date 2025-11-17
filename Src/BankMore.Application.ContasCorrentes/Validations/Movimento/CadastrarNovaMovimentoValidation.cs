using BankMore.Application.ContasCorrentes.Commands;

namespace BankMore.Application.ContasCorrentes.Validations;

public class CadastrarNovaMovimentacaoValidation : MovimentoValidation<CadastrarNovaMovimentacaoCommand>
{
    public CadastrarNovaMovimentacaoValidation()
    {
        ValidarId();
        ValidarIdContaCorrente();
        ValidarDataMovimento();
        ValidarValor();
        ValidarTipoMovimento();
    }
}
