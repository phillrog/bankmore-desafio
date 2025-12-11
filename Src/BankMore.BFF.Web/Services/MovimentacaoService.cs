using BankMore.BFF.Web.ViewModels;
using BankMore.Domain.Common;
using BankMore.Domain.Common.Dtos;

namespace BankMore.BFF.Web.Services
{
    public interface IMovimentacaoService
    {
        Task<Response<MovimentacaoRelaizadaViewModel>> Debito(MovimentoViewModel vmodel);
    }

    public class MovimentacaoService : IMovimentacaoService
    {
        private readonly HttpClient _downstreamClient;
        private const string BaseApi = "/api/v1/Movimento";
        public MovimentacaoService(IHttpClientFactory httpClientFactory)
        {
            _downstreamClient = httpClientFactory.CreateClient("ContasCorrentesAPI");
        }
      

        public async Task<Response<MovimentacaoRelaizadaViewModel>> Debito(MovimentoViewModel vmodel)
        {
            vmodel.TipoMovimento = "D";
            
            var response = await _downstreamClient.PostAsJsonAsync($"{BaseApi}", vmodel);

            return await response.Content.ReadFromJsonAsync<Response<MovimentacaoRelaizadaViewModel>>();
        }
    }
}
