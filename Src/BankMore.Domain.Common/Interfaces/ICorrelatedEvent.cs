using MediatR;

namespace BankMore.Domain.Common.Interfaces
{
    public interface ICorrelatedEvent : INotification
    {
        Guid CorrelationId { get; }
    }
}
