using BankMore.BFF.Web.ViewModels;

namespace BankMore.BFF.Web.Services
{
    public interface ITransferenciaService
    {
        Task<Response<TransferenciaViewModel>> Transferir(RealizarTransferenciaViewModel vmodel);
    }

    public class TransferenciaService : ITransferenciaService
    {
        private readonly HttpClient _downstreamClient;
        private const string BaseApi = "/api/v1/Transferencia";
        public TransferenciaService(IHttpClientFactory httpClientFactory)
        {
            _downstreamClient = httpClientFactory.CreateClient("TransferenciasAPI");
        }
      

        public async Task<Response<TransferenciaViewModel>> Transferir(RealizarTransferenciaViewModel vmodel)
        {           
            var response = await _downstreamClient.PostAsJsonAsync($"{BaseApi}", vmodel);

            return await response.Content.ReadFromJsonAsync<Response<TransferenciaViewModel>>();
        }
    }
}
