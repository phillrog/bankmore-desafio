using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Kafka.Events;
using BankMore.Infra.Kafka.Producers;
using BankMore.Infra.Kafka.Responses;
using KafkaFlow;

namespace BankMore.Infra.Kafka.Services
{
    public class InformacoesContaService
    {
        private readonly IMessageProducer<IInforcacoesContaRequestProducer> _requestProducer;
        private readonly NumeroContaReplyManager _responseManager;
        private const string ReplyTopic = "informacoes.conta.resposta"; 

        public InformacoesContaService(IMessageProducer<IInforcacoesContaRequestProducer> requestProducer,
            NumeroContaReplyManager responseManager,
            IInformacoesContaRespository informacoesContaRespository)
        {
            _requestProducer = requestProducer;
            _responseManager = responseManager;
        }

        public async Task<Result<int>> ObterNumeroContaPorCpf(string cpf)
        {
            var correlationId = Guid.NewGuid();
            var requestEvent = new BuscarNumeroContaEvent(cpf, correlationId, ReplyTopic);

            await _requestProducer.ProduceAsync("informacoes.conta.requisicao", correlationId.ToString(), requestEvent); 

            return await _responseManager.WaitForResponseAsync(correlationId);
        }

        public async Task<Result<int>> ObterNumeroContaPorNumero(int numero)
        {
            var correlationId = Guid.NewGuid();
            var requestEvent = new BuscarNumeroContaEvent(numero, correlationId, ReplyTopic);

            await _requestProducer.ProduceAsync("informacoes.conta.requisicao", correlationId.ToString(), requestEvent);

            return await _responseManager.WaitForResponseAsync(correlationId);
        }
    }
}
