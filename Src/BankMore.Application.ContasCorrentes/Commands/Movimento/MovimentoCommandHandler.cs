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

public class MovimentoCommandHandler : CommandHandler,
    IRequestHandler<CadastrarNovaMovimentacaoCommand, Result<MovimentacaoRelaizadaDto>>
{
    #region [ SERVICES ]

    private readonly IMediatorHandler _bus;
    private readonly IMovimentoRepository _movimentoRepository;
    #endregion

    #region [ CONSTRUTOR ]

    public MovimentoCommandHandler(
        IUnitOfWork uow,
        IMediatorHandler bus,
        INotificationHandler<DomainNotification> notifications,
        IMovimentoRepository movimentoRepository)
        : base(uow, bus, notifications)
    {
        _bus = bus;
        _movimentoRepository = movimentoRepository;
    }
    #endregion

    #region [ HANDLERS ]       

    public Task<Result<MovimentacaoRelaizadaDto>> Handle(CadastrarNovaMovimentacaoCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid())
        {
            NotifyValidationErrors(message);
            return Task.FromResult(Result<MovimentacaoRelaizadaDto>.Failure(message, Erro.INVALID_DOCUMENT));
        }
        
        if (_movimentoRepository.GetById(message.Id) != null)
        {
            var erro = "Movimentação já cadastrada.";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Task.FromResult(Result<MovimentacaoRelaizadaDto>.Failure(erro, Erro.INVALID_DOCUMENT));
        }

        var movimento = new Movimento();

        _movimentoRepository.Add(movimento);
        if (Commit())
        {
            _bus.RaiseEvent(new MovimentoRegistradoEvent(movimento.Id, movimento.IdContaCorrente, movimento.DataMovimento, movimento.TipoMovimento, movimento.Valor));
            return Task.FromResult(Result<MovimentacaoRelaizadaDto>.Success(new MovimentacaoRelaizadaDto
            {
                Id = movimento.Id,
                NumeroConta = 0,
                DataHora = movimento.DataMovimento,
                Tipo = movimento.TipoMovimento,
                Valor = movimento.Valor
            }));
        }

        return Task.FromResult(Result<MovimentacaoRelaizadaDto>.Failure("Ops! Algo deu errado ao salvar movimento", Erro.INVALID_TYPE));
    }


    #endregion
}
