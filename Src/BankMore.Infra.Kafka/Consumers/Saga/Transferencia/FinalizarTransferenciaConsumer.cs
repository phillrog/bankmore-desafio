using BankMore.Application.Transferencias.Commands;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Domain.Common.Events;
using KafkaFlow;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Infra.Kafka.Consumers
{
    public class FinalizarTransferenciaConsumer : IMessageHandler<FinalizarTransferenciaCommand>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public FinalizarTransferenciaConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Handle(IMessageContext context, FinalizarTransferenciaCommand command)
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var mediator = serviceProvider.GetRequiredService<IMediator>();
                var dispatcher = serviceProvider.GetRequiredService<IValidarDebitoProducerDispatcher>();

                MovimentacaoContaRespostaEvent resposta = null;

                try
                {
                    resposta = await mediator.Send(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERRO] Falha ao executar o comando FinalizarTransferenciaConsumer: {ex.Message}");
                    return;
                }
            }
        }
    }
}