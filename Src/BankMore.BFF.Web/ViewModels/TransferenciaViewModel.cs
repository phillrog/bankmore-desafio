using BankMore.Domain.Common.Enums;

namespace BankMore.BFF.Web.ViewModels
{
    public class TransferenciaViewModel
    {
        public Guid Id { get; set; }
        public StatusEnum Status { get; set; }
        public string StatusDescricao { get; set; }
        public DateTime DataAceitacao { get; set; }
        public decimal Valor { get; set; }
        public int NumeroContaDestino { get; set; }
    }
}
