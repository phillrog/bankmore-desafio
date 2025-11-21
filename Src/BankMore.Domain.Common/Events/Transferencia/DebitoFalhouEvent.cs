using BankMore.Domain.Core.Events;

namespace BankMore.Domain.Common.Events
{
    public class DebitoFalhouEvent : Event
    {
        public DebitoFalhouEvent(Guid id, string erro)
        {
            Id = id;
            DataAlteracao = DateTime.UtcNow;
            Erro = erro;
        }

        public Guid Id { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Erro { get; set; }
    }
}
