using BankMore.Domain.ContasCorrentes.Enums;
using BankMore.Domain.Core.Commands;

namespace BankMore.Application.ContasCorrentes.Commands;

public abstract class MovimentoCommand : Command
{
    public Guid Id { get; set; }
    public Guid IdContaCorrente { get; set; }
    public DateTime DataMovimento { get; set; }
    public TipoMovimento TipoMovimento { get; set; }
    public decimal Valor { get; set; }
}
