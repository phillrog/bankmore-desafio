using BankMore.Domain.Core.Events;

namespace BankMore.Domain.ContasCorrentes.Events;

public class IdempotenciaRegistradoEvent : Event
{
    public IdempotenciaRegistradoEvent(Guid id, Guid idContaCorrente, string requisicao)
    {
        Id = id;
        IdContaCorrente = idContaCorrente;
        Requisicao = requisicao;
    }

    public Guid Id { get; private set; }
    public Guid IdContaCorrente { get; private set; }
    public string Requisicao { get; private set; }    
}
