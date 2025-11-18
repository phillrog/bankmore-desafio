using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Models;
using BankMore.Infra.Kafka.Events.Movimento;
using KafkaFlow;
using System.Collections.Concurrent;

namespace BankMore.Infra.Kafka.Responses
{
    public class MovimentacaoReplyManager : IMessageHandler<MovimentacaoResponseEvent>
    {
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<Result<MovimentacaoRelaizadaDto>>> _pendingRequests = new();

        private readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);
        public Task<Result<MovimentacaoRelaizadaDto>> WaitForResponseAsync(Guid correlationId)
        {
            var tcs = new TaskCompletionSource<Result<MovimentacaoRelaizadaDto>>();
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

        public Task Handle(IMessageContext context, MovimentacaoResponseEvent message)
        {
            if (_pendingRequests.TryRemove(message.CorrelationId, out var tcs))
            {
                Result<MovimentacaoRelaizadaDto> operationResult;

                if (message.IsSuccess)
                {
                    operationResult = Result<MovimentacaoRelaizadaDto>.Success(new MovimentacaoRelaizadaDto()
                    {
                        DataHora = message.DataHora,
                        Id = message.Id,
                        Nome = message.Nome,
                        NumeroConta = message.NumeroConta,
                        SaldoAposMovimentacao = message.SaldoAposMovimentacao,
                        Tipo = message.Tipo,
                        Valor = message.Valor
                    });
                }
                else
                {
                    operationResult = Result<MovimentacaoRelaizadaDto>.Failure(message.ErrorMessage, message.ErrorType);
                }

                tcs.TrySetResult(operationResult);
            }

            return Task.CompletedTask;
        }
    }
}
