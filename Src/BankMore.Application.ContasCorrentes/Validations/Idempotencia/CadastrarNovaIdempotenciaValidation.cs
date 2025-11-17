using BankMore.Application.ContasCorrentes.Commands;

namespace BankMore.Application.ContasCorrentes.Validations;

public class CadastrarNovaIdempotenciaValidation : IdempotenciaValidation<CadastrarNovaIdempotenciaCommand>
{
    public CadastrarNovaIdempotenciaValidation()
    {
        ValidarId();
        ValidarRequisicao();
    }
}
