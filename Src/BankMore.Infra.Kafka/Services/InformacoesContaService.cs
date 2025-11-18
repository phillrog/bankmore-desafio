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
            NumeroContaReplyManager responseManager)
        {
            _requestProducer = requestProducer;
            _responseManager = responseManager;
        }

        public async Task<Result<int>> BuscarNumeroConta(string cpf)
        {
            var correlationId = Guid.NewGuid();
            var requestEvent = new BuscarNumeroContaEvent(cpf, correlationId, ReplyTopic);

            await _requestProducer.ProduceAsync("informacoes.conta.requisicao", cpf, requestEvent); 

            return await _responseManager.WaitForResponseAsync(correlationId);
        }
    }
}
