namespace BankMore.Domain.ContasCorrentes.Dtos
{
    /// <summary>
    /// Data Transfer Object (DTO) que representa o resumo de saldo de uma conta corrente.
    /// Utilizado para retornar informaçÃµes detalhadas de crédito, débito e saldo atualizado.
    /// </summary>
    public class SaldoDto
    {
        public SaldoDto() { }

        /// <summary>
        /// Construtor para inicialização completa do SaldoDto.
        /// </summary>
        /// <param name="totalCredito">O valor total de créditos (entradas) na conta.</param>
        /// <param name="totalDebito">O valor total de débitos (saÃ­das) na conta.</param>
        /// <param name="saldoAtualizado">O saldo atual da conta (Créditos - Débitos).</param>
        /// <param name="numeroConta">O nÃºmero Ãºnico da conta corrente.</param>
        public SaldoDto(decimal totalCredito, decimal totalDebito, decimal saldoAtualizado, int numeroConta)
        {
            TotalCredito = totalCredito;
            TotalDebito = totalDebito;
            SaldoAtualizado = saldoAtualizado;
            NumeroConta = numeroConta;
        }

        /// <summary>
        /// O valor total acumulado de todas as transaçÃµes de crédito.
        /// </summary>
        public decimal TotalCredito { get; set; }

        /// <summary>
        /// O valor total acumulado de todas as transaçÃµes de débito.
        /// </summary>
        public decimal TotalDebito { get; set; }

        /// <summary>
        /// O saldo final calculado (TotalCrédito - TotalDébito).
        /// </summary>
        public decimal SaldoAtualizado { get; set; }

        /// <summary>
        /// Verifica se o saldo atualizado é menor que um valor especÃ­fico.
        /// </summary>
        /// <param name="valor">O valor a ser comparado com o saldo atual.</param>
        /// <returns>True se o saldo for insuficiente, False caso contrário.</returns>
        public bool SaldoInsuficiente(decimal valor) => SaldoAtualizado < valor;

        /// <summary>
        /// O nÃºmero da conta corrente ao qual o saldo se refere.
        /// </summary>
        public int? NumeroConta { get; set; }
    }
}