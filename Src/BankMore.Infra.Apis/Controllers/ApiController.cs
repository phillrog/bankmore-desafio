using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Core.Notifications;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Services.Apis.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ApiController : ControllerBase
{
    #region [ SERVICES ]

    private readonly DomainNotificationHandler _notifications;
    private readonly IMediatorHandler _mediator;
    #endregion

    #region [ CONSTRUTOR]

    protected ApiController(
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler mediator)
    {
        _notifications = (DomainNotificationHandler)notifications;
        _mediator = mediator;
    }
    #endregion

    #region [ PROTECTEDS ]


    protected IEnumerable<DomainNotification> Notifications => _notifications.GetNotifications();

    protected bool IsValidOperation()
    {
        return !_notifications.HasNotifications();
    }

    protected IActionResult ResponseResult(dynamic result)
    {        
        if (result.IsSuccess)
        {
            return Ok(new
            {
                success = true,
                data = result.Data,
            });
        }
       
        return BadRequest(new
        {
            success = false,
            data = result.Data,
            errors = (result as dynamic)?.Erros ?? new List<string>(),
            errorType = result.ErroTipo
        });
    }

    protected new IActionResult Response(object result = null)
    {
        if (IsValidOperation())
        {
            return Ok(new
            {
                success = true,
                data = result,
            });
        }

        return BadRequest(new
        {
            success = false,
            errors = _notifications.GetNotifications().Select(n => n.Value),
        });
    }

    protected void NotifyModelStateErrors()
    {
        var erros = ModelState.Values.SelectMany(v => v.Errors);
        foreach (var erro in erros)
        {
            var erroMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
            NotifyError(string.Empty, erroMsg);
        }
    }

    protected void NotifyError(string code, string message)
    {
        _mediator.RaiseEvent(new DomainNotification(code, message));
    }

    protected void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            NotifyError(result.ToString(), error.Description);
        }
    }

    protected void AddError(string error, string description)
    {
        NotifyError(error, description);        
    }

    #endregion
}

