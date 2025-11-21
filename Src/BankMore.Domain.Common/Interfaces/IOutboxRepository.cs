using BankMore.Domain.Common.Wrappers;
using MediatR;

namespace BankMore.Domain.Common.Interfaces
{
    public interface IOutboxRepository
    {
        void Add(INotification message);
        Task<IEnumerable<OutboxMessageWrapper>> GetPendingMessagesAsync(int take);
        Task MarkAsProcessedAsync(IEnumerable<OutboxMessageWrapper> messages);
        Task<bool> ExistTransfer(Guid chaveIdempotencia);
    }
}
