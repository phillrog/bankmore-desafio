
using BankMore.Application.ContasCorrentes.Commands;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Events;

using KafkaFlow;

using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Application.ContasCorrentes.Consumers;

public class UsuarioCriadoHandler : IMessageHandler<UsuarioCriadoEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UsuarioCriadoHandler(IServiceScopeFactory serviceScopeFactory)
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
