using AutoMapper;
using BankMore.Domain.Common.CommandHandlers;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Core.Notifications;
using BankMore.Domain.Transferencias.Interfaces;
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

    public async Task<Result<bool>> Handle(CadastrarNovaIdempotenciaCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid())
        {
            NotifyValidationErrors(message);
            return Result<bool>.Failure(message, Erro.INVALID_DOCUMENT);
        }

        var idempotencia = await _idempotenciaRepository.GetByExpressionAsync(d => d.Id == message.Id);
        if (idempotencia is not null)
        {
            var erro = "Chave já cadastrada.";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Result<bool>.Failure(erro, Erro.INVALID_DOCUMENT);
        }

        var chave = _mapper.Map<BankMore.Domain.Transferencias.Models.Idempotencia>(message);

        _idempotenciaRepository.Add(chave);
        if (Commit())
        {
            return Result<bool>.Success(true);
        }

        return Result<bool>.Failure("Ops! Algo deu errado ao salvar idempotencia", Erro.INVALID_TYPE);
    }


    #endregion
}
