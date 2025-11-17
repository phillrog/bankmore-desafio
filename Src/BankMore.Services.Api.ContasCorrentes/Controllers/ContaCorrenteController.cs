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
public class ContaCorrenteController : ApiController
{
    #region [ SERVICES ]
    private readonly IContaCorrenteService _contaCorrenteService;
    #endregion

    #region [ CONSTRUTOR ]

    public ContaCorrenteController(
        IContaCorrenteService contaCorrenteService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler mediator)
        : base(notifications, mediator)
    {
        _contaCorrenteService = contaCorrenteService;
    }
    #endregion

    #region [ GET ]
    /// <summary>
    /// Consulta as informações básicas de uma conta corrente pelo CPF do titular.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite acesso não autenticado (AllowAnonymous).
    /// Retorna o nome do titular, número e status da conta.
    /// </remarks>
    /// <param name="cpf">O CPF do titular da conta.</param>
    /// <returns>Retorna os dados da conta (Nome, Número, Ativo) ou um erro.</returns>
    [HttpGet("informacoes/{cpf}")]
    [ProducesResponseType(typeof(InformacoesViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Informacoes(string cpf)
    {
        var result = await _contaCorrenteService.BuscarInformcoes(cpf);        

        return ResponseResult(result);
    }
    #endregion

    #region [ POST ]
    /// <summary>
    /// Cadastra uma nova conta corrente no sistema.
    /// </summary>
    /// <remarks>
    /// Requer autenticação com a Role 'Master' e a Policy 'CanWriteData'.
    /// </remarks>
    /// <param name="contaViewModel">Dados da conta a ser cadastrada.</param>
    /// <returns>Retorna a conta cadastrada em caso de sucesso.</returns>
    [HttpPost]
    [Authorize(Policy = "CanWriteData", Roles = Roles.Master)]
    [ProducesResponseType(typeof(ContaCorrenteViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Cadastrar([FromBody] ContaCorrenteViewModel contaViewModel)
    {
        if (!ModelState.IsValid)
        {
            NotifyModelStateErrors();
            return Response(contaViewModel);
        }

        _contaCorrenteService.Cadastrar(contaViewModel);

        return Response(contaViewModel);
    }
    #endregion

    #region [ PUT ]
    /// <summary>
    /// Altera os dados de uma conta corrente existente.
    /// </summary>
    /// <remarks>
    /// O ID deve ser fornecido no corpo da requisição. Requer autenticação com a Role 'Admin' e a Policy 'CanWriteData'.
    /// </remarks>
    /// <param name="contaViewModel">Dados da conta a ser alterada.</param>
    /// <returns>Retorna a conta alterada em caso de sucesso.</returns>
    [HttpPut]
    [Authorize(Policy = "CanWriteData", Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ContaCorrenteViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Alterar([FromBody] ContaCorrenteViewModel contaViewModel)
    {
        if (!ModelState.IsValid)
        {
            NotifyModelStateErrors();
            return Response(contaViewModel);
        }

        _contaCorrenteService.Alterar(contaViewModel);

        return Response();
    }
    #endregion
}
