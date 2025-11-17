namespace BankMore.Domain.ContasCorrentes.Dtos
{
    public class MovimentacaoRelaizadaDto
    {
        public Guid? Id { get; set; }
        public int? NumeroConta { get; set; }
        public string Nome { get; set; }
        public DateTime? DataHora { get; set; }
        public char Tipo { get; set; }
        public decimal? Valor { get; set; }
    }
}
