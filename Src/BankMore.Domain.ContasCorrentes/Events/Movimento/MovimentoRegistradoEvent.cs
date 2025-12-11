using BankMore.Domain.Core.Events;

namespace BankMore.Domain.ContasCorrentes.Events;

public class MovimentoRegistradoEvent : Event
{
    public MovimentoRegistradoEvent(Guid id, Guid idContaCorrente, DateTime dataMovimento, char tipoMovimento, decimal valor, string descricao)
    {
        Id = id;
        IdContaCorrente = idContaCorrente;
        DataMovimento = dataMovimento;
        TipoMovimento = tipoMovimento;
        Valor = valor;
        Descricao = descricao;
    }

    public Guid Id { get; private set; }
    public Guid IdContaCorrente { get; private set; }
    public DateTime DataMovimento { get; private set; }
    public char TipoMovimento { get; private set; }
    public decimal Valor { get; private set; }
    public string Descricao { get; set; }
}
