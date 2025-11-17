using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Commands;
using BankMore.Domain.Core.Events;
using MediatR;

namespace BankMore.Infra.CrossCutting.Bus;

public sealed class InMemoryBus : IMediatorHandler
{
    private readonly IMediator _mediator;

    public InMemoryBus(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task SendCommand<T>(T command)
        where T : Command
    {
        return _mediator.Send(command);
    }

    public Task<TResult> SendCommand<T, TResult>(T command) where T : IRequest<TResult>
    {
        return _mediator.Send(command);
    }

    public Task RaiseEvent<T>(T @event)
        where T : Event
    {
        return _mediator.Publish(@event);
    }
}
