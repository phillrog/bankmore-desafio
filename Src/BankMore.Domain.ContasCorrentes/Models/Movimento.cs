using BankMore.Domain.Core.Models;

namespace BankMore.Domain.ContasCorrentes.Models
{
    public class Movimento : EntityAudit
    {
        public Movimento() { }
        public Movimento(Guid id, Guid idContaCorrente, DateTime dataMovimento, char tipoMovimento, decimal valor)
        {
            IdContaCorrente = idContaCorrente;
            DataMovimento = dataMovimento;
            TipoMovimento = tipoMovimento;
            Valor = valor;
            Id = id;
        }

        public Guid IdContaCorrente { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public char TipoMovimento { get; private set; }
        public decimal Valor { get; private set; }
        public Guid? IdTransferencia { get; private set; }

        public void DefinirIdTransferencia(Guid id)
        {
            IdTransferencia = id;
        }
        public ContaCorrente? ContaCorrente { get; set; }
    }
}
