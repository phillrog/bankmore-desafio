using BankMore.Application.Common.Topicos.Saga;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Common.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankMore.Transferencias.Workers
{
    public class OutboxMessageRelayer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxMessageRelayer> _logger;
        private const int PollingDelayMs = 100; // Intervalo de busca (ex: 100 milissegundos)
        private const int BatchSize = 100;      // Quantidade máxima de mensagens por ciclo

        public OutboxMessageRelayer(
            IServiceProvider serviceProvider,
            ILogger<OutboxMessageRelayer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox Relayer iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(PollingDelayMs, stoppingToken);
                await ProcessOutboxMessages(stoppingToken);
            }
            _logger.LogInformation("Outbox Relayer encerrado.");
        }

        private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
        {
            // Usamos um service scope para garantir que as dependências (como DbContext/IOutboxRepository) 
            // sejam resolvidas corretamente para o ciclo de vida desta busca.
            using var scope = _serviceProvider.CreateScope();

            // Injeção dos serviços necessários
            var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
            // Assumindo que você tem um serviço para publicar no Kafka
            var sagaProducerService = scope.ServiceProvider.GetRequiredService<ISagaTransferenciaProducerDispatcher>();

            var successfullySent = new List<OutboxMessageWrapper>();

            try
            {
                // 1. BUSCA (Polling)
                // Usa o método que consulta a tabela OutboxMessages onde IsProcessed = 0
                var pendingMessages = await outboxRepo.GetPendingMessagesAsync(BatchSize);

                if (!pendingMessages.Any()) return;

                _logger.LogInformation("Encontradas {Count} mensagens pendentes no Outbox.", pendingMessages.Count());

                // 2. ENVIO (Publicação no Kafka)
                foreach (var message in pendingMessages)
                {
                    if (stoppingToken.IsCancellationRequested) return;

                    try
                    {
                        // 💡 Assumindo que o sagaProducerService tem um método PublishAsync
                        // Ele usará o Type e o Payload do evento para montar a mensagem
                        await sagaProducerService.PublishAsync(message.Event, SagaTopico.IniciarTranferencia);

                        successfullySent.Add(message);
                    }
                    catch (Exception publishEx)
                    {
                        _logger.LogError(publishEx, $"Falha ao publicar mensagem Outbox ID: {(message as dynamic)?.Id}.");
                    }
                }

                // 3. MARCAÇÃO (Limpeza)
                // Atualiza IsProcessed = 1 para as mensagens que saíram com sucesso.
                if (successfullySent.Any())
                {
                    await outboxRepo.MarkAsProcessedAsync(successfullySent);
                    _logger.LogInformation("Marcadas {Count} mensagens como processadas no Outbox.", successfullySent.Count);
                }
            }
            catch (Exception dbEx)
            {
                // Falha na Conexão com o DB (o OutboxRepo falhou no Get ou Mark)
                _logger.LogCritical(dbEx, "Erro crítico de banco de dados no Outbox Relayer.");
                // Retornar e esperar o próximo ciclo.
            }
        }
    }
}
