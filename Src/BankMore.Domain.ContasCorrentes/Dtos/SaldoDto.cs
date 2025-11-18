namespace BankMore.Domain.ContasCorrentes.Dtos
{
    public class SaldoDto
    {
        public SaldoDto(){}
        public SaldoDto(decimal totalCredito, decimal totalDebito, decimal saldoAtualizado, int numeroConta)
        {
            TotalCredito = totalCredito;
            TotalDebito = totalDebito;
            SaldoAtualizado = saldoAtualizado;
            NumeroConta = numeroConta;
        }

        public decimal TotalCredito { get; set; }
        public decimal TotalDebito { get; set; }
        public decimal SaldoAtualizado { get; set; }
        public bool SaldoInsuficiente(decimal valor) => SaldoAtualizado < valor;
        public int? NumeroConta { get; set; }
    }
}
