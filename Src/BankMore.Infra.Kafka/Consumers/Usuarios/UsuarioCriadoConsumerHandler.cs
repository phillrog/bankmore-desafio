using BankMore.Application.ContasCorrentes.Commands;
using BankMore.Domain.Core.Bus;
using BankMore.Infra.Kafka.Events;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Infra.Kafka.Consumers;

public class UsuarioCriadoConsumerHandler : IMessageHandler<UsuarioCriadoEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UsuarioCriadoConsumerHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    public async Task Handle(IMessageContext context, UsuarioCriadoEvent message)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();

        var bus = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
        var registerCommand = new CadastrarNovaContaCorrenteCommand(message.Id, message.Nome, message.Senha, message.Cpf);
        await bus.SendCommand(registerCommand);
    }
}
