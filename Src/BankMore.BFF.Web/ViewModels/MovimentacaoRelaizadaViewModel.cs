namespace BankMore.BFF.Web.ViewModels
{
    public class MovimentacaoRelaizadaViewModel
    {
        public Guid Id { get; set; }
        public int NumeroConta { get; set; }
        public string Nome { get; set; }
        public DateTime DataHora { get; set; }
        public char Tipo { get; set; }
        public decimal Valor { get; set; }
        public decimal SaldoAposMovimentacao { get; set; }
        public string Descricao { get; set; }
        
    }
}
