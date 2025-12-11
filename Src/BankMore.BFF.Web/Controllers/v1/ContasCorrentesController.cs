using BankMore.Domain.Common;
using BankMore.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.BFF.Web.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ContasCorrentesController : ControllerBase
    {
        private readonly HttpClient _downstreamClient;

        public ContasCorrentesController(IHttpClientFactory httpClientFactory)
        {
            _downstreamClient = httpClientFactory.CreateClient("ContasCorrentesAPI");
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
            var claimNumeroConta = User.FindFirst("numero_conta");

            if (claimNumeroConta == null)
            {
                return Unauthorized("A claim 'numero_conta' não foi encontrada.");
            }

            string numeroConta = claimNumeroConta.Value;

            var response = await _downstreamClient.GetAsync($"/api/v1/ContaCorrente/informacoes?numeroConta={numeroConta}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
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
            var claimNumeroConta = User.FindFirst("numero_conta");

            if (claimNumeroConta == null)
            {
                return Unauthorized("A claim 'numero_conta' não foi encontrada.");
            }

            string numeroConta = claimNumeroConta.Value;

            var response = await _downstreamClient.GetAsync($"/api/v1/ContaCorrente/saldo?numeroConta={numeroConta}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}
