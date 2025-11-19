using BankMore.Application.Transferencias.Commands;

namespace BankMore.Application.Transferencias.Validations;

public class CadastrarNovaIdempotenciaValidation : IdempotenciaValidation<CadastrarNovaIdempotenciaCommand>
{
    public CadastrarNovaIdempotenciaValidation()
    {
        ValidarId();
        ValidarRequisicao();
    }
}
