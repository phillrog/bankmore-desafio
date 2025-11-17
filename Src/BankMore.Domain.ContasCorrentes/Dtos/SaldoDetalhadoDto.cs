namespace BankMore.Domain.ContasCorrentes.Dtos
{
    public class SaldoDetalhadoDto
    {
        public decimal TotalCredito { get; set; }
        public decimal TotalDebito { get; set; }
        public decimal SaldoAtualizado { get; set; }
        public bool SaldoInsuficiente(decimal valor) => SaldoAtualizado < valor;
    }
}
