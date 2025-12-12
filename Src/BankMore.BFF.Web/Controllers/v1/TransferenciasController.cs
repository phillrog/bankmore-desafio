using BankMore.BFF.Web.Extensions;
using BankMore.BFF.Web.Services;
using BankMore.BFF.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BankMore.BFF.Web.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class TransferenciasController : ControllerBase
    {
        private readonly ITransferenciaService _transferenciaService;
        private readonly IContaCorrenteService _contaCorrenteService;

        public TransferenciasController(ITransferenciaService transferenciaService, 
            IContaCorrenteService contaCorrenteService)
        {
            _transferenciaService = transferenciaService;
            _contaCorrenteService = contaCorrenteService;
        }

        /// <summary>
        /// 💸 Realiza uma nova transferência de valor entre a conta do usuário autenticado e uma conta destino.
        /// </summary>
        /// <remarks>
        /// Esta operação inicia a saga de transferência, que envolve verificação de saldo, criação de movimentos e atualização de saldos.
        /// O resultado deve ser monitorado.
        /// </remarks>
        /// <param name="realizarTransferenciaViewModel">Dados necessários para a transferência: Valor, Conta Destino e Senha da conta de origem.</param>
        /// <returns>Retorna os dados da transferência registrada (incluindo seu ID) e o status da operação.</returns>
        [HttpPost("transferir")]
        [ProducesResponseType(typeof(RealizarTransferenciaViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostCadastrar([FromBody] RealizarTransferenciaViewModel realizarTransferenciaViewModel)
        {
            string? numeroConta = this.GetNumeroConta();

            if (numeroConta == null)
            {
                return Unauthorized("A claim 'numero_conta' não foi encontrada.");
            }

            realizarTransferenciaViewModel.NumeroContaCorrenteOrigem = numeroConta;

            var saldo = await _contaCorrenteService.BuscarSaldo(numeroConta);

            if (saldo.Data.SaldoInsuficiente(realizarTransferenciaViewModel.Valor))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Saldo insuficiente");
            }

            var response = await _transferenciaService.Transferir(realizarTransferenciaViewModel);

            if (response.Success)
            {
                return Ok(response);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }
    }
}
