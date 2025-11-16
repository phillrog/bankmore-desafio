using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Commands;
using BankMore.Domain.Core.Notifications;
using MediatR;

namespace BankMore.Domain.Common.CommandHandlers;

public class CommandHandler
{
    private readonly IUnitOfWork _uow;
    private readonly IMediatorHandler _bus;
    private readonly DomainNotificationHandler _notifications;

    public CommandHandler(IUnitOfWork uow, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications)
    {
        _uow = uow;
        _notifications = (DomainNotificationHandler)notifications;
        _bus = bus;
    }

    public bool Commit()
    {
        if (_notifications.HasNotifications())
        {
            return false;
        }

        if (_uow.Commit())
        {
            return true;
        }

        _bus.RaiseEvent(new DomainNotification("Commit", "Ops! Ocorreu um problema ao salvar os dados."));
        return false;
    }

    protected void NotifyValidationErrors(Command message)
    {
        foreach (var error in message.ValidationResult.Errors)
        {
            _bus.RaiseEvent(new DomainNotification(message.MessageType, error.ErrorMessage));
        }
    }
}
