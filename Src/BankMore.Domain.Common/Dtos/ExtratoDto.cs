
namespace BankMore.Domain.Common.Dtos
{
    /// <summary>
    /// Representa uma única movimentação (débito ou crédito) no extrato da conta corrente.
    /// </summary>
    public class ExtratoDto
    {
        /// <summary>
        /// ID único da movimentação (da tabela 'movimento').
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Data e hora exata em que a movimentação ocorreu.
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// O valor total da transação.
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Tipo de movimentação: "CRÉDITO" (entrada) ou "DÉBITO" (saída).
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// O nome da conta de contraparte (origem do crédito ou destino do débito).
        /// Ex: Nome do titular da conta que enviou o dinheiro.
        /// </summary>
        public string NomeContraparte { get; set; } 

        /// <summary>
        /// O número da conta de origem da transação (para Créditos) ou o número da conta atual (para Débitos).
        /// </summary>
        public int NumeroContaOrigem { get; set; }

        /// <summary>
        /// O número da conta de destino da transação (para Débitos) ou o número da conta atual (para Créditos).
        /// </summary>
        public int NumeroContaDestino { get; set; }
    }
}
