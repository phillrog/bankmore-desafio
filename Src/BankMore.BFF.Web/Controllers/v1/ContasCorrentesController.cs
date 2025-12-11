using BankMore.BFF.Web.Extensions;
using BankMore.BFF.Web.Services;
using BankMore.BFF.Web.ViewModels;
using BankMore.Domain.Common;
using BankMore.Domain.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BankMore.BFF.Web.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class ContasCorrentesController : ControllerBase
    {
        private IContaCorrenteService _contaCorrenteService;
        private IMovimentacaoService _movimentacaoService;

        public ContasCorrentesController(IContaCorrenteService contaCorrenteService, IMovimentacaoService movimentacaoService)
        {
            _contaCorrenteService = contaCorrenteService;
            _movimentacaoService = movimentacaoService;
        }

        /// <summary>
        /// Consulta as informações básicas de uma conta corrente.
        /// </summary>
        /// <remarks>
        /// O sistema usará o número 
        /// da conta do usuário logado (extraído do token JWT).
        /// </remarks>
        /// <returns>Retorna os dados da conta (Nome, Número, Ativo) ou um erro.</returns>
        [HttpGet("informacoes")]
        [ProducesResponseType(typeof(InformacoesContaCorrenteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInformacoes()
        {
            string? numeroConta = this.GetNumeroConta();

            if (numeroConta == null)
            {
                return Unauthorized("A claim 'numero_conta' não foi encontrada.");
            }

            var response = await _contaCorrenteService.BuscarInformacoes(numeroConta);

            if (response.Success)
            {
                return Ok(response);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }

        /// <summary>
        /// Consulta o saldo atual e os totais de crédito/débito de uma conta corrente.
        /// </summary>
        /// <remarks>
        /// Permite a consulta do próprio saldo ou o saldo de terceiros (para Masters/Admins).
        /// </remarks>                
        /// <returns>Retorna um SaldoDto contendo o saldo atualizado e os totais de crédito/débito, ou um erro.</returns>
        [HttpGet("saldo")]
        [ProducesResponseType(typeof(InformacoesContaCorrenteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSaldo()
        {
            string? numeroConta = this.GetNumeroConta();

            if (numeroConta == null)
            {
                return Unauthorized("A claim 'numero_conta' não foi encontrada.");
            }

            var response = await _contaCorrenteService.BuscarSaldo(numeroConta);

            if (response.Success)
            {
                return Ok(response);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }

        /// <summary>
        /// Gera o extrato de movimentações (débitos e créditos) de uma conta corrente.
        /// </summary>
        /// <remarks>
        /// Permite a consulta do próprio extrato ou o extrato de terceiros (para Masters/Admins).
        /// </remarks>
        /// <returns>Retorna uma lista de ExtratoDto contendo todas as movimentações.</returns>
        [HttpGet("extrato")]
        [ProducesResponseType(typeof(IEnumerable<ExtratoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExtrato()
        {
            string? numeroConta = this.GetNumeroConta();

            if (numeroConta == null)
            {
                return Unauthorized("A claim 'numero_conta' não foi encontrada.");
            }

            var response = await _contaCorrenteService.BuscarExtrato(numeroConta);

            if (response.Success)
            {
                return Ok(response);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }

        /// <summary>
        /// Registra uma nova movimentação débito na conta corrente.
        /// </summary>
        /// <remarks>
        /// A requisição é processada assincronamente via Kafka. 
        /// O número da conta é extraído do token JWT do usuário logado, garantindo que o usuário só 
        /// movimente sua própria conta, a menos que seja um administrador.
        /// </remarks>
        /// <param name="contaViewModel">Os dados da movimentação, incluindo tipo (crédito/débito) e valor.</param>
        /// <returns>Retorna o resultado da submissão da mensagem para o Kafka ou erros de validação.</returns>
        [HttpPost("debitar")]
        [ProducesResponseType(typeof(MovimentoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PostDebitar([FromBody] MovimentoViewModel contaViewModel)
        {
            string? numeroConta = this.GetNumeroConta();

            if (numeroConta == null)
            {
                return Unauthorized("A claim 'numero_conta' não foi encontrada.");
            }

            contaViewModel.Conta = numeroConta;

            var response = await _movimentacaoService.Debito(contaViewModel);

            if (response.Success)
            {
                return Ok(response);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }
    }
}
