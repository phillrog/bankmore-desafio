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


namespace BankMore.Services.Api.ContasCorrentes.Controllers.V1;

[Route("api/v{version:apiVersion}/[controller]")]
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

    /// <summary>
    /// Registra uma nova movimentação (crédito ou débito) na conta corrente.
    /// </summary>
    /// <remarks>
    /// A requisição é processada assincronamente via Kafka. 
    /// O número da conta é extraído do token JWT do usuário logado, garantindo que o usuário só 
    /// movimente sua própria conta, a menos que seja um administrador.
    /// Requer a **Role 'Admin'** e a **Policy 'CanWriteData'**.
    /// </remarks>
    /// <param name="contaViewModel">Os dados da movimentação, incluindo tipo (crédito/débito) e valor.</param>
    /// <param name="user">Objeto de usuário injetado para extrair o número da conta do token.</param>
    /// <returns>Retorna o resultado da submissão da mensagem para o Kafka ou erros de validação.</returns>
    [HttpPost]
    [Authorize(Policy = "CanWriteData", Roles = Roles.Admin)]
    [ProducesResponseType(typeof(MovimentoViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PostCadastrar([FromBody] MovimentoViewModel contaViewModel, [FromServices] IUser user)
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
