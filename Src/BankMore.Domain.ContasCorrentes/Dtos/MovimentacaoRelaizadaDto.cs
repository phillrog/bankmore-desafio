namespace BankMore.Domain.ContasCorrentes.Dtos
{
    public class MovimentacaoRelaizadaDto
    {
        public Guid Id { get; set; }
        public int NumeroConta { get; set; }
        public string Nome { get; set; }
        public DateTime DataHora { get; set; }
        public char Tipo { get; set; }
        public decimal Valor { get; set; }
        public decimal SaldoAposMovimentacao { get; set;  }

        public MovimentacaoRelaizadaDto() {}
        public MovimentacaoRelaizadaDto(Guid id, int numeroConta, string nome, DateTime dataHora, char tipo, decimal valor, decimal saldoAposMovimentacao)
        {
            Id = id;
            NumeroConta = numeroConta;
            Nome = nome;
            DataHora = dataHora;
            Tipo = tipo;
            Valor = valor;
            SaldoAposMovimentacao = saldoAposMovimentacao;
        }
    }
}
