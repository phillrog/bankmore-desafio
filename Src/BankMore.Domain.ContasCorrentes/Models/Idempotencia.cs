using BankMore.Domain.Core.Models;

namespace BankMore.Domain.ContasCorrentes.Models
{
    public class Idempotencia : EntityAudit
    {
        public Guid Id { get; set; }
        public Guid IdContaCorrente { get; set; }
        public string Requisicao { get; set; }
        public string Resultado { get; set; }

        public ContaCorrente? ContaCorrente { get; set; }
    }
}
