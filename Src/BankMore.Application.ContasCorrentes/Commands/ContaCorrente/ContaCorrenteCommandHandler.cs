using BankMore.Domain.Common.CommandHandlers;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Events;
using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.ContasCorrentes.Models;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Core.Notifications;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Commands;

public class ContaCorrenteCommandHandler : CommandHandler,
    IRequestHandler<CadastrarNovaContaCorrenteCommand, Result<NumeroContaCorrenteDto>>,
    IRequestHandler<AlterarContaCorrenteCommand, bool>
{
    #region [ SERVICES ]

    private readonly IContaCorrenteRepository _contaCorrenteRepository;
    private readonly IGeradorNumeroService _geradorNumeroService;
    private readonly IMediatorHandler _bus;
    #endregion

    #region [ CONSTRUTOR ]

    public ContaCorrenteCommandHandler(
        IContaCorrenteRepository contaCorrenteRepository,
        IGeradorNumeroService geradorNumeroService,
        IUnitOfWork uow,
        IMediatorHandler bus,
        INotificationHandler<DomainNotification> notifications)
        : base(uow, bus, notifications)
    {
        _contaCorrenteRepository = contaCorrenteRepository;
        _geradorNumeroService = geradorNumeroService;
        _bus = bus;
    }
    #endregion

    #region [ HANDLERS ]       

    public Task<Result<NumeroContaCorrenteDto>> Handle(CadastrarNovaContaCorrenteCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid())
        {
            NotifyValidationErrors(message);
            return Task.FromResult(Result<NumeroContaCorrenteDto>.Failure(message, Erro.INVALID_DOCUMENT));
        }

        if (_contaCorrenteRepository.GetByCpf(message.Cpf) != null)
        {
            var erro = "Já existe uma conta com este CPF cadastrado.";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Task.FromResult(Result<NumeroContaCorrenteDto>.Failure(erro, Erro.INVALID_DOCUMENT));
        }

        var conta = new ContaCorrente(message.Id ?? Guid.NewGuid(), message.Nome, _geradorNumeroService.GerarNumeroConta(), message.Cpf, message.Senha);
        _contaCorrenteRepository.Add(conta);

        if (Commit())
        {
            _bus.RaiseEvent(new ContaCorrenteRegistradoEvent(conta.Id, conta.Nome, conta.Numero, conta.Cpf, conta.Senha));
            return Task.FromResult(Result<NumeroContaCorrenteDto>.Success(new NumeroContaCorrenteDto(conta.Numero)));
        }

        return Task.FromResult(Result<NumeroContaCorrenteDto>.Failure(new List<string> { "Ops! Algo deu errado" }, Erro.INVALID_TYPE));
    }

    public Task<bool> Handle(AlterarContaCorrenteCommand message, CancellationToken cancellationToken)
    {
        #region [ VALIDAÇÕES ]

        if (!message.IsValid())
        {
            NotifyValidationErrors(message);
            return Task.FromResult(false);
        }

        var conta = _contaCorrenteRepository.GetByNumero(message.Numero);

        if (conta is null)
        {
            _bus.RaiseEvent(new DomainNotification(message.MessageType, "Conta corrente inválido."));
            return Task.FromResult(false);
        }

        /// Alterar senha
        if (message.SenhaAnterior is not null)
        {
            /// Calcula o salt atual
            var id = conta.Id.ToString().ToLower();
            var saltAnterior = message.SenhaAnterior + id;

            if (!_geradorNumeroService.ValidarSenha(conta.Senha, saltAnterior))
            {
                _bus.RaiseEvent(new DomainNotification(message.MessageType, "Senha anterior inválida."));
                return Task.FromResult(false);
            }

            var novaSenha = _geradorNumeroService.GerarSenha(message.Senha + id);

            conta.AtualizarSenha(novaSenha);
        }
        #endregion

        conta.Status(message.Ativo);
        conta.AtualizarNome(message.Nome);

        _contaCorrenteRepository.Update(conta);

        if (Commit())
        {
            _bus.RaiseEvent(new ContaCorrenteAlteradoEvent(conta.Id, conta.Nome, conta.Numero, conta.Cpf, conta.Senha, conta.Ativo));
        }

        return Task.FromResult(true);
    }

    #endregion
}
