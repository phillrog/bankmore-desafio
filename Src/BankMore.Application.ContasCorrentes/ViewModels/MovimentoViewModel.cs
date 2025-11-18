
using BankMore.Domain.ContasCorrentes.Enums;
using System.Text.Json.Serialization;

namespace BankMore.Application.ContasCorrentes.ViewModels
{
    /// <summary>
    /// Representa os dados de uma movimentação financeira (transação) associada a uma conta corrente.
    /// </summary>
    public class MovimentoViewModel
    {       
        /// <summary>
        /// Tipo de operação realizada. 'C' para Crédito (entrada) ou 'D' para Débito (saída).
        /// </summary>
        /// <example>C</example>
        /// 
        public TipoMovimento TipoMovimento { get; set; }

        /// <summary>
        /// Valor monetário da transação.
        /// </summary>
        /// <example>500.25</example>
        public decimal Valor { get; set; }

        /// <summary>
        /// Número da conta do usuário
        /// </summary>
        /// <example>123456</example>
        [JsonIgnore]
        public string Conta { get; set; }
    }
}
