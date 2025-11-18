using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Notifications;
using BankMore.Infra.CrossCutting.Identity.Authorization;
using BankMore.Infra.Kafka.Services;
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
    private readonly MovimentarContaService _movimentarKafkaService;

    #endregion

    #region [ CONSTRUTOR ]

    public MovimentoController(
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler mediator,
        MovimentarContaService movimentarKafkaService,
        IUser user)
        : base(notifications, mediator)
    {
        _movimentarKafkaService = movimentarKafkaService;
    }
    #endregion
    
    #region [ POST ]
    
    [HttpPost]
    [Authorize(Policy = "CanWriteData", Roles = Roles.Admin)]
    [ProducesResponseType(typeof(MovimentoViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] MovimentoViewModel contaViewModel, [FromServices] IUser user)
    {
        if (!ModelState.IsValid)
        {
            NotifyModelStateErrors();
            return Response(contaViewModel);
        }
        contaViewModel.Conta = user.Conta;
        var retorno = await _movimentarKafkaService.Movimentar(contaViewModel);

        return ResponseResult(retorno);
    }
    #endregion

}
