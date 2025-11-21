using BankMore.Domain.Common.Enums;
using BankMore.Domain.Core.Models;

namespace BankMore.Domain.Transferencias.Models
{
    public class Transferencia : EntityAudit
    {
        public Guid IdContaCorrenteOrigem { get; set; }
        public Guid IdContaCorrenteDestino { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal Valor { get; set; }
        public StatusEnum Status { get; set; }
        public string StatusDescricao { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public string Erro { get; set; }

        public bool Processou() => new[] { StatusEnum.CONCLUIDA, StatusEnum.ERRO_DEBITO, StatusEnum.ERRO_DEBITO }.Contains(Status);

        public void AtualizarContaOrigem(Guid id) => IdContaCorrenteOrigem = id;
        public void AtualizarContaDestino(Guid id) => IdContaCorrenteDestino = id;
        public void AtualizarStatus(StatusEnum status)
        {
            Status = status;
            StatusDescricao = Status.ToString();
        }
        public void AtualizarDataUltimaAlteracao(DateTime data) => DataUltimaAlteracao = data;
    }
}
