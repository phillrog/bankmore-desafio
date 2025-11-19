using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Notifications;
using BankMore.Infra.Kafka.Services;
using BankMore.Services.Apis.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace BankMore.Services.Api.ContasCorrentes.Controllers.V2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
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
    [HttpGet("saldo")]
    public IActionResult Saldo()
    {
        return Ok(new { Versao = "2.0", Saldo = 1500.00m, NomeCliente = "João Silva" });
    }

    #endregion
}
