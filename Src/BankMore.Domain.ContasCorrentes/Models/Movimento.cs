using BankMore.Domain.Core.Models;

namespace BankMore.Domain.ContasCorrentes.Models
{
    public class Movimento : EntityAudit
    {
        public Guid IdContaCorrente { get; set; }
        public DateTime DataMovimento { get; set; }
        public char TipoMovimento { get; set; }
        public decimal Valor { get; set; }
        public ContaCorrente? ContaCorrente { get; set; }
    }
}
