
using BankMore.Domain.ContasCorrentes.Enums;

namespace BankMore.Application.ContasCorrentes.ViewModels
{
    /// <summary>
    /// Representa os dados de uma movimentação financeira (transação) associada a uma conta corrente.
    /// </summary>
    public class MovimentoViewModel
    {
        /// <summary>
        /// ID da Movimentação.
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid Id { get; set; }

        /// <summary>
        /// ID da Conta Corrente à qual este movimento pertence.
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid IdContaCorrente { get; set; }

        /// <summary>
        /// Data e hora exatas em que o movimento foi registrado.
        /// </summary>
        /// <example>2025-11-17T11:00:00Z</example>
        public DateTime DataMovimento { get; set; }

        /// <summary>
        /// Tipo de operação realizada. 'C' para Crédito (entrada) ou 'D' para Débito (saída).
        /// </summary>
        /// <example>C</example>
        public TipoMovimento TipoMovimento { get; set; }

        /// <summary>
        /// Valor monetário da transação.
        /// </summary>
        /// <example>500.25</example>
        public decimal Valor { get; set; }
    }
}
