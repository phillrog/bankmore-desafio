using BankMore.Domain.Core.Commands;
using BankMore.Domain.Core.Events;
using MediatR;

namespace BankMore.Domain.Core.Bus;

public interface IMediatorHandler
{
    Task SendCommand<T>(T command)
        where T : Command;

    Task<TResult> SendCommand<T, TResult>(T command)
        where T : IRequest<TResult>;

    Task RaiseEvent<T>(T @event)
        where T : Event;
}
