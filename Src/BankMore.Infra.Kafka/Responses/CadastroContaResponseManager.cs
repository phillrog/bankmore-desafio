using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Kafka.Events.ContaCorrente;
using KafkaFlow;
using System.Collections.Concurrent;

namespace BankMore.Infra.Kafka.Responses
{
    public class CadastroContaResponseManager : IMessageHandler<CadastrarContaCorrenteResponseEvent>
    {
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<Result<NumeroContaCorrenteDto>>> _pendingRequests = new();

        private readonly TimeSpan Timeout = TimeSpan.FromSeconds(300);
        public Task<Result<NumeroContaCorrenteDto>> WaitForResponseAsync(Guid correlationId)
        {
            var tcs = new TaskCompletionSource<Result<NumeroContaCorrenteDto>>();
            _pendingRequests.TryAdd(correlationId, tcs);

            _ = Task.Delay(Timeout).ContinueWith(t =>
            {
                if (_pendingRequests.TryRemove(correlationId, out var pendingTcs))
                {
                    pendingTcs.TrySetException(new TimeoutException($"Resposta do Kafka para o CorrelationId {correlationId} excedeu o tempo limite."));
                }
            });

            return tcs.Task;
        }

        public Task Handle(IMessageContext context, CadastrarContaCorrenteResponseEvent message)
        {
            if (_pendingRequests.TryRemove(message.CorrelationId, out var tcs))
            {
                Result<NumeroContaCorrenteDto> operationResult;

                if (message.IsSuccess)
                {
                    operationResult = Result<NumeroContaCorrenteDto>.Success(new NumeroContaCorrenteDto(message.NumeroConta));
                }
                else
                {
                    operationResult = Result<NumeroContaCorrenteDto>.Failure(message.ErrorMessage, message.ErrorType);
                }

                tcs.TrySetResult(operationResult);
            }

            return Task.CompletedTask;
        }
    }
}
