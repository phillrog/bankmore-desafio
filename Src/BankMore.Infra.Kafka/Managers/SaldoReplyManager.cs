using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Kafka.Events.ContaCorrente;
using KafkaFlow;
using System.Collections.Concurrent;

namespace BankMore.Infra.Kafka.Managers
{
    public class SaldoReplyManager : IMessageHandler<SaldoResponseEvent>
    {
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<Result<SaldoDto>>> _pendingRequests = new();

        private readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);
        public Task<Result<SaldoDto>> WaitForResponseAsync(Guid correlationId)
        {
            var tcs = new TaskCompletionSource<Result<SaldoDto>>();
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

        public Task Handle(IMessageContext context, SaldoResponseEvent message)
        {
            if (_pendingRequests.TryRemove(message.CorrelationId, out var tcs))
            {
                Result<SaldoDto> operationResult;

                if (message.IsSuccess)
                {
                    operationResult = Result<SaldoDto>.Success(new SaldoDto(message.TotalCredito, message.TotalDebito, message.Saldo, message.NumeroConta));
                }
                else
                {
                    operationResult = Result<SaldoDto>.Failure(message.ErrorMessage, message.ErrorType);
                }

                tcs.TrySetResult(operationResult);
            }

            return Task.CompletedTask;
        }
    }
}