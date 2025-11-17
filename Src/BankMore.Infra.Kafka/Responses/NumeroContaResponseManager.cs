using BankMore.Domain.Core.Models;
using BankMore.Infra.Kafka.Events.ContaCorrente;
using KafkaFlow;
using System.Collections.Concurrent;

namespace BankMore.Infra.Kafka.Responses
{
    public class NumeroContaResponseManager : IMessageHandler<NumeroContaEncontradoResponseEvent>
    {
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<Result<int>>> _pendingRequests = new();

        private readonly TimeSpan Timeout = TimeSpan.FromSeconds(300);
        public Task<Result<int>> WaitForResponseAsync(Guid correlationId)
        {
            var tcs = new TaskCompletionSource<Result<int>>();
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

        public Task Handle(IMessageContext context, NumeroContaEncontradoResponseEvent message)
        {
            if (_pendingRequests.TryRemove(message.CorrelationId, out var tcs))
            {
                tcs.TrySetResult(Result<int>.Success(message.NumeroConta));
            }

            return Task.CompletedTask;
        }
    }
}
