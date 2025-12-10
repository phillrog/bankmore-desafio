using BankMore.Domain.Common;
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
        public async Task<IActionResult> GetInformacoes([FromServices] IHttpContextAccessor httpContextAccessor)
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
    }
}
