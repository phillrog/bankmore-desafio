using BankMore.Domain.Common;
using BankMore.Domain.Common.Dtos;

namespace BankMore.BFF.Web.Services
{
    public interface IMovimentacaoService
    {
        Task<Response<InformacoesContaCorrenteDto>> BuscarInformacoes(string numeroConta);
        Task<Response<SaldoDto>> BuscarSaldo(string numeroConta);
        Task<Response<IEnumerable<ExtratoDto>>> BuscarExtrato(string numeroConta);
    }

    public class MovimentacaoService : IMovimentacaoService
    {
        private readonly HttpClient _downstreamClient;
        private const string BaseApi = "/api/v1/ContaCorrente";
        public MovimentacaoService(IHttpClientFactory httpClientFactory)
        {
            _downstreamClient = httpClientFactory.CreateClient("ContasCorrentesAPI");
        }
        public async Task<Response<IEnumerable<ExtratoDto>>> BuscarExtrato(string numeroConta)
        {
            var response = await _downstreamClient.GetAsync($"{BaseApi}/extrato?numeroConta={numeroConta}");

            return await response.Content.ReadFromJsonAsync<Response<IEnumerable<ExtratoDto>>>();
        }

        public async Task<Response<InformacoesContaCorrenteDto>> BuscarInformacoes(string numeroConta)
        {
            var response = await _downstreamClient.GetAsync($"{BaseApi}/informacoes?numeroConta={numeroConta}");

            return await response.Content.ReadFromJsonAsync<Response<InformacoesContaCorrenteDto>>();
        }

        public async Task<Response<SaldoDto>> BuscarSaldo(string numeroConta)
        {
            var response = await _downstreamClient.GetAsync($"{BaseApi}/saldo?numeroConta={numeroConta}");

            return await response.Content.ReadFromJsonAsync<Response<SaldoDto>>();
        }
    }
}
