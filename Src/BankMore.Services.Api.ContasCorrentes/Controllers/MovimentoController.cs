using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Notifications;
using BankMore.Infra.CrossCutting.Identity.Authorization;
using BankMore.Services.Apis.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BankMore.Services.Api.ContasCorrentes.Controllers;

[Route("api/[controller]")]
[ApiVersion("1.0")]
public class MovimentoController : ApiController
{
    #region [ SERVICES ]
    private readonly IMovimentoService _movimentoService;
    #endregion

    #region [ CONSTRUTOR ]

    public MovimentoController(
        IMovimentoService movimentoService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler mediator)
        : base(notifications, mediator)
    {
        _movimentoService = movimentoService;
    }
    #endregion

    #region [ GET ]
    
    #endregion

    #region [ POST ]
    
    [HttpPost]
    [Authorize(Policy = "CanWriteData", Roles = Roles.Admin)]
    [ProducesResponseType(typeof(MovimentoViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Cadastrar([FromBody] MovimentoViewModel contaViewModel)
    {
        if (!ModelState.IsValid)
        {
            NotifyModelStateErrors();
            return Response(contaViewModel);
        }

        _movimentoService.Cadastrar(contaViewModel);

        return Response(contaViewModel);
    }
    #endregion

    #region [ PUT ]
   
    #endregion
}
