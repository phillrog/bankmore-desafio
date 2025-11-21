using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Common;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Notifications;
using BankMore.Infra.Apis.Configurations;
using BankMore.Infra.Kafka.Services;
using BankMore.Services.Apis.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Services.Api.ContasCorrentes.Controllers.V1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ContaCorrenteController : ApiController
{
    #region [ SERVICES ]
    private readonly IContaCorrenteService _contaCorrenteService;
    private readonly SaldoService _saldoService;
    #endregion

    #region [ CONSTRUTOR ]

    public ContaCorrenteController(
        IContaCorrenteService contaCorrenteService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler mediator,
        SaldoService saldoService)
        : base(notifications, mediator)
    {
        _contaCorrenteService = contaCorrenteService;
        _saldoService = saldoService;
    }
    #endregion

    #region [ GET ]
    /// <summary>
    /// Consulta as informações básicas de uma conta corrente.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite que usuários autenticados consultem sua própria conta ou, 
    /// se possuírem a Role Master/Admin e a Policy 'OwnerOrMaster_Conta', consultem a conta de terceiros 
    /// passando o parâmetro `numeroConta`. Se o `numeroConta` for omitido, o sistema usará o número 
    /// da conta do usuário logado (extraído do token JWT).
    /// </remarks>
    /// <param name="numeroConta">O número da conta a ser consultada (opcional, se omitido usa a conta do token).</param>
    /// <param name="user">Objeto de usuário injetado para obter dados do token.</param>
    /// <returns>Retorna os dados da conta (Nome, Número, Ativo) ou um erro.</returns>
    [Authorize(Policy = "OwnerOrMaster_Conta")]
    [HttpGet("informacoes")]
    [ProducesResponseType(typeof(InformacoesContaCorrenteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetInformacoes([FromQuery] int? numeroConta, [FromServices] IUser user)
    {
        var conta = numeroConta ?? Convert.ToInt32(user.Conta);

        if (conta == 0)
        {
            return Response("Número conta não foi encontrado");
        }

        var result = await _contaCorrenteService.BuscarInformcoes(conta);

        return ResponseResult(result);
    }

    /// <summary>
    /// Consulta o saldo atual e os totais de crédito/débito de uma conta corrente.
    /// </summary>
    /// <remarks>
    /// Permite a consulta do próprio saldo ou o saldo de terceiros (para Masters/Admins).
    /// Se `numeroConta` for omitido, a consulta é feita na conta do usuário logado.
    /// </remarks>
    /// <param name="numeroConta">O número da conta cujo saldo será consultado (opcional).</param>
    /// <param name="user">Objeto de usuário injetado para obter dados do token.</param>
    /// <returns>Retorna um SaldoDto contendo o saldo atualizado e os totais de crédito/débito, ou um erro.</returns>
    [Authorize(Policy = "OwnerOrMaster_Conta")]
    [HttpGet("saldo")]
    [ProducesResponseType(typeof(InformacoesContaCorrenteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSaldo([FromQuery] int? numeroConta, [FromServices] IUser user)
    {
        var conta = numeroConta ?? Convert.ToInt32(user.Conta);

        if (conta == 0)
        {
            return Response("Número conta não foi encontrado");
        }

        var result = await _saldoService.ConsultarSaldo(conta);

        return ResponseResult(result);
    }
    #endregion

    #region [ POST ]
    /// <summary>
    /// Cadastra uma nova conta corrente no sistema.
    /// </summary>
    /// <remarks>
    /// Requer autenticação.
    /// </remarks>
    /// <param name="contaViewModel">Dados da conta a ser cadastrada (inclui informações do titular e iniciais da conta).</param>
    /// <returns>Retorna a conta cadastrada em caso de sucesso.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(NovaCorrenteViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PolicySetup.CanWriteDataOrMasterPolicy)]
    public IActionResult PostCadastrar([FromBody] NovaCorrenteViewModel contaViewModel)
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
    /// Este endpoint é usado para modificações administrativas na conta.
    /// </remarks>
    /// <param name="contaViewModel">Dados completos da conta a ser alterada. O ID ou número da conta deve ser fornecido no corpo do objeto.</param>
    /// <returns>Retorna um status de sucesso (200 OK) ou erro.</returns>
    [Authorize(Policy = "CanWriteData", Roles = "Master,Admin")]
    [HttpPut]
    [ProducesResponseType(typeof(ContaCorrenteViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult PutAlterar([FromBody] ContaCorrenteViewModel contaViewModel)
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
