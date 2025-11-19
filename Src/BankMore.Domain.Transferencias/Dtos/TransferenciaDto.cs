using BankMore.Domain.Transferencias.Enums;

namespace BankMore.Domain.Transferencias.Dtos
{
    public class TransferenciaDto
    {
        public Guid Id { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.PENDENTE;
        public DateTime DataAceitacao { get; set; } = DateTime.UtcNow;
        public decimal Valor { get; set; }
        public int NumeroContaDestino { get; set; }
    }
}
