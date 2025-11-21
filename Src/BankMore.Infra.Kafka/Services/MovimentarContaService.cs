using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Kafka.Events.Movimento;
using BankMore.Infra.Kafka.Responses;
using BankMore.Infra.Kafka.Tags;
using KafkaFlow;

namespace BankMore.Infra.Kafka.Services
{
    public class MovimentarContaService
    {
        private readonly IMessageProducer<IMovimentacaoRequestProducerTag> _requestProducer;
        private readonly MovimentacaoReplyManager _responseManager;
        private const string ReplyTopic = "movimentar.conta.resposta";

        public MovimentarContaService(IMessageProducer<IMovimentacaoRequestProducerTag> requestProducer,
            MovimentacaoReplyManager responseManager)
        {
            _requestProducer = requestProducer;
            _responseManager = responseManager;
        }

        public async Task<Result<MovimentacaoRelaizadaDto>> Movimentar(MovimentoViewModel message)
        {
            var correlationId = Guid.NewGuid();
            var requestEvent = new MovimentacaoRequestEvent()
            {
                Valor = message.Valor,
                Tipo = message.TipoMovimento,
                CorrelationId = correlationId,
                ReplyTopic = ReplyTopic,
                Conta = message.Conta,
            };

            await _requestProducer.ProduceAsync("movimentar.conta.requisicao", correlationId.ToString(), requestEvent);

            return await _responseManager.WaitForResponseAsync(correlationId);
        }
    }
}
