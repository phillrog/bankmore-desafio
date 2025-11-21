using MediatR;

namespace BankMore.Domain.Common.Wrappers
{
    public class OutboxMessageWrapper
    {
        public Guid OutboxId { get; set; }
        public INotification Event { get; set; }
    }
}
