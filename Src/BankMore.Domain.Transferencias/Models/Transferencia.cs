using BankMore.Domain.Core.Models;
using BankMore.Domain.Transferencias.Enums;

namespace BankMore.Domain.Transferencias.Models
{
    public class Transferencia : EntityAudit
    {       
        public Guid IdContaCorrenteOrigem { get; set; }
        public Guid IdContaCorrenteDestino { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal Valor { get; set; }
        public StatusEnum Status { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public string Erro { get; set; }

        public void AtualizarContaOrigem(Guid id) => IdContaCorrenteOrigem = id;
        public void AtualizarContaDestino(Guid id) => IdContaCorrenteDestino = id;
        public void AtualizarStatus(StatusEnum status) => Status = status;
        public void AtualizarDataUltimaAlteracao(DateTime data) => DataUltimaAlteracao = data;
    }
}
