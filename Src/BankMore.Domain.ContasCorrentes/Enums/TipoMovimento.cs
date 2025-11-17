using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankMore.Domain.ContasCorrentes.Enums
{
    /// <summary>
    /// Define o tipo de movimentação financeira (transação) realizada na conta.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoMovimento
    {
        /// <summary>
        /// Crédito: Representa uma entrada de valor na conta.
        /// </summary>
        [Display(Name = "Crédito")]
        Credito = 'C',

        /// <summary>
        /// Débito: Representa uma saída de valor da conta.
        /// </summary>
        [Display(Name = "Débito")]
        Debito = 'D'
    }
}
