using BankMore.Application.Transferencias.Commands;

namespace BankMore.Application.Transferencias.Validations
{
    public class RealizarTransferenciaValidation : TransferenciaValidation<TransferenciaCommand>
    {
        public RealizarTransferenciaValidation()
        {
            ValidarId();
            ValidarDataMovimento();
            ValidarValor();
            ValidarNumeroContaOrigem();
            ValidarNumeroContaDestino();
        }
    }
}