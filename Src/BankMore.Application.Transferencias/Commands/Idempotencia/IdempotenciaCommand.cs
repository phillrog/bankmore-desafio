using BankMore.Domain.Core.Commands;

namespace BankMore.Application.Transferencias.Commands;

public abstract class IdempotenciaCommand : Command
{
    public Guid Id { get; protected set; }
    public Guid IdContaCorrente { get; protected set; }
    public string Requisicao { get; protected set; }
    public string Resultado { get; protected set; }
}
