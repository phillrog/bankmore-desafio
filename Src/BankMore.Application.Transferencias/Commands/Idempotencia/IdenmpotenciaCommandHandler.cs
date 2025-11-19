using AutoMapper;
using BankMore.Domain.Common.CommandHandlers;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Transferencias.Interfaces;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Core.Notifications;
using MediatR;


namespace BankMore.Application.Transferencias.Commands;

public class IdenmpotenciaCommandHandler : CommandHandler,
    IRequestHandler<CadastrarNovaIdempotenciaCommand, Result<bool>>
{
    #region [ SERVICES ]

    private readonly IMediatorHandler _bus;
    private readonly IIdempotenciaRepository _idempotenciaRepository;
    private readonly IMapper _mapper;
    #endregion

    #region [ CONSTRUTOR ]

    public IdenmpotenciaCommandHandler(
        IUnitOfWork uow,
        IMediatorHandler bus,
        INotificationHandler<DomainNotification> notifications,
        IIdempotenciaRepository idempotenciaRepository,
        IMapper mapper)
        : base(uow, bus, notifications)
    {
        _bus = bus;
        _idempotenciaRepository = idempotenciaRepository;
        _mapper = mapper;
    }
    #endregion

    #region [ HANDLERS ]       

    public Task<Result<bool>> Handle(CadastrarNovaIdempotenciaCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid())
        {
            NotifyValidationErrors(message);
            return Task.FromResult(Result<bool>.Failure(message, Erro.INVALID_DOCUMENT));
        }

        if (_idempotenciaRepository.GetById(message.Id) != null)
        {
            var erro = "Chave já cadastrada.";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Task.FromResult(Result<bool>.Failure(erro, Erro.INVALID_DOCUMENT));
        }

        var chave = _mapper.Map<BankMore.Domain.Transferencias.Models.Idempotencia>(message);

        _idempotenciaRepository.Add(chave);
        if (Commit())
        {
            return Task.FromResult(Result<bool>.Success(true));
        }

        return Task.FromResult(Result<bool>.Failure("Ops! Algo deu errado ao salvar idempotencia", Erro.INVALID_TYPE));
    }


    #endregion
}
