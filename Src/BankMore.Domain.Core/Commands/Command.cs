using BankMore.Domain.Core.Events;

using FluentValidation.Results;

namespace BankMore.Domain.Core.Commands;

public abstract class Command : Message
{
    public DateTime Timestamp { get; private set; }

    public ValidationResult ValidationResult { get; set; } = new ValidationResult();

    protected Command()
    {
        Timestamp = DateTime.Now;
    }

    public abstract bool IsValid();
}
