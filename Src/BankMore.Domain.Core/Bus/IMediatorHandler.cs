using System.Threading.Tasks;

using BankMore.Domain.Core.Commands;
using BankMore.Domain.Core.Events;

namespace BankMore.Domain.Core.Bus;

public interface IMediatorHandler
{
    Task SendCommand<T>(T command)
        where T : Command;

    Task RaiseEvent<T>(T @event)
        where T : Event;
}
