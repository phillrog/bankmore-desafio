using BankMore.Domain.Common.Dtos;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Kafka.Events.ContaCorrente;
using BankMore.Infra.Kafka.Managers;
using BankMore.Infra.Kafka.Tags;
using KafkaFlow;

namespace BankMore.Infra.Kafka.Services
{
    public class SaldoService : ISaldoService
    {
        private readonly IMessageProducer<ISaldoRequestProducerTag> _requestProducer;
        private readonly SaldoReplyManager _responseManager;
        private const string ReplyTopic = "buscar-saldo.conta.resposta";

        public SaldoService(IMessageProducer<ISaldoRequestProducerTag> requestProducer,
            SaldoReplyManager responseManager)
        {
            _requestProducer = requestProducer;
            _responseManager = responseManager;
        }

        public async Task<Result<SaldoDto>> ConsultarSaldo(int numero)
        {
            var correlationId = Guid.NewGuid();
            var requestEvent = new SaldoRequestEvent()
            {
                NumeroConta = numero,
                CorrelationId = correlationId,
                ReplyTopic = ReplyTopic,
            };

            await _requestProducer.ProduceAsync("buscar-saldo.conta.requisicao", correlationId.ToString(), requestEvent);

            return await _responseManager.WaitForResponseAsync(correlationId);
        }
    }
}
