
using BankMore.Domain.Core.Models;

namespace BankMore.Domain.Transferencias.Models
{
    public class Idempotencia : EntityAudit
    {
        public Idempotencia(){}
        public Idempotencia(Guid id, Guid idContaCorrente, string requisicao, string resultado)
        {
            Id = id;
            IdContaCorrente = idContaCorrente;
            Requisicao = requisicao;
            Resultado = resultado;
        }

        public Guid Id { get; private set; }
        public Guid IdContaCorrente { get; private set; }
        public string Requisicao { get; private set; }
        public string Resultado { get; private set; }
    }
}
