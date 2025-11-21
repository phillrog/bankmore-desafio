using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Kafka.Events;
using BankMore.Infra.Kafka.Events.ContaCorrente;
using BankMore.Infra.Kafka.Responses;
using BankMore.Infra.Kafka.Tags;
using KafkaFlow;

namespace BankMore.Infra.Kafka.Services
{
    public class ContaCorrenteService
    {
        private readonly IMessageProducer<ICadastroContaRequestProducerTag> _requestProducer;
        private readonly CadastroContaReplyManager _responseManager;
        private const string ReplyTopic = "cadastrar.conta.resposta";

        public ContaCorrenteService(IMessageProducer<ICadastroContaRequestProducerTag> requestProducer,
            CadastroContaReplyManager responseManager)
        {
            _requestProducer = requestProducer;
            _responseManager = responseManager;
        }

        public async Task<Result<NumeroContaCorrenteDto>> CadastrarConta(UsuarioCriadoEvent message)
        {
            var correlationId = Guid.NewGuid();
            var requestEvent = new CadastrarContaCorrenteRequestEvent(message.Id, message.Nome, message.Cpf, message.Senha, correlationId, ReplyTopic);

            await _requestProducer.ProduceAsync("cadastrar.conta.requisicao", correlationId.ToString(), requestEvent);

            return await _responseManager.WaitForResponseAsync(correlationId);
        }
    }
}
