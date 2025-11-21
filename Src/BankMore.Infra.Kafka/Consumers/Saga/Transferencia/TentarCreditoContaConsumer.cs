using BankMore.Application.Common.Commands;
using BankMore.Application.Common.Topicos.Saga;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Domain.Common.Events;
using KafkaFlow;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Infra.Kafka.Consumers
{
    public class TentarCreditoContaConsumer : IMessageHandler<TentarCreditoCommand>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TentarCreditoContaConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Handle(IMessageContext context, TentarCreditoCommand message)
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var mediator = serviceProvider.GetRequiredService<IMediator>();
                var dispatcher = serviceProvider.GetRequiredService<IValidarDebitoProducerDispatcher>();

                MovimentacaoContaRespostaEvent resposta = null;

                try
                {
                    resposta = await mediator.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERRO] Falha ao executar o comando TentarCreditoCommandHandler: {ex.Message}");
                    return;
                }

                if (resposta != null)
                {
                    await dispatcher.PublishAsync(
                        resposta,
                        SagaTopico.ValidarDebitoConta
                    );
                }
            }
        }
    }
}
