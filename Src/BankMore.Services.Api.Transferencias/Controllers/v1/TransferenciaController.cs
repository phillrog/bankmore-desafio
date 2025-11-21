using BankMore.Application.Transferencias.Interfaces;
using BankMore.Application.Transferencias.ViewModels;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Notifications;
using BankMore.Infra.Apis.Configurations;
using BankMore.Services.Apis.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BankMore.Services.Api.Transferencias.Controllers.V1;

/// <summary>
/// Gerencia a criação e o processamento de novas transferências entre contas correntes.
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
// [ApiController(Name = "Transferências")])
public class TransferenciaController : ApiController
{
    #region [ SERVICES ]
    private readonly ITransferenciaService _transferenciasService;
    #endregion

    #region [ CONSTRUTOR ]

    public TransferenciaController(
        ITransferenciaService TransferenciasService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler mediator)
        : base(notifications, mediator)
    {
        _transferenciasService = TransferenciasService;
    }
    #endregion

    #region [ POST ]

    /// <summary>
    /// 💸 Realiza uma nova transferência de valor entre a conta do usuário autenticado e uma conta destino.
    /// </summary>
    /// <remarks>
    /// Esta operação inicia a saga de transferência, que envolve verificação de saldo, criação de movimentos e atualização de saldos.
    /// O resultado deve ser monitorado.
    /// </remarks>
    /// <param name="realizarTransferenciaViewModel">Dados necessários para a transferência: Valor, Conta Destino e Senha da conta de origem.</param>
    /// <returns>Retorna os dados da transferência registrada (incluindo seu ID) e o status da operação.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RealizarTransferenciaViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PolicySetup.CanWriteDataOrMasterPolicy)]
    public async Task<IActionResult> PostCadastrar([FromBody] RealizarTransferenciaViewModel realizarTransferenciaViewModel)
    {
        if (!ModelState.IsValid)
        {
            NotifyModelStateErrors();
            return Response(realizarTransferenciaViewModel);
        }

        var result = await _transferenciasService.Cadastrar(realizarTransferenciaViewModel);

        return ResponseResult(result);
    }

    #endregion
}