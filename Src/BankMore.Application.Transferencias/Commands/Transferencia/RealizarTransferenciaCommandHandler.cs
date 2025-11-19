using AutoMapper;
using BankMore.Domain.Common.CommandHandlers;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Core.Notifications;
using BankMore.Domain.Transferencias.Dtos;
using BankMore.Infra.Kafka.Services;
using MediatR;

namespace BankMore.Application.Transferencias.Commands
{
    public class RealizarTransferenciaCommandHandler : CommandHandler,
        IRequestHandler<TransferenciaCommand, Result<TransferenciaDto>>
    {
        #region [ SERVICES ]

        private readonly IMediatorHandler _bus;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private readonly InformacoesContaService _informacoesContaService;
        #endregion

        #region [ CONSTRUTOR ]

        public RealizarTransferenciaCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,            
            IUser user,
            IMapper mapper,
            InformacoesContaService informacoesContaService)
            : base(uow, bus, notifications)
        {
            _bus = bus;
            _user = user;
            _mapper = mapper;
            _informacoesContaService = informacoesContaService;
        }

        #endregion
        public async Task<Result<TransferenciaDto>> Handle(TransferenciaCommand message, CancellationToken cancellationToken)
        {
            #region [ VALIDAÇÕES ]
            if (!message.IsValid())
            {
                NotifyValidationErrors(message);
                return Result<TransferenciaDto>.Failure(message, Erro.INVALID_DOCUMENT);
            }

            var contaOrigem = await _informacoesContaService.ObterNumeroContaPorNumero(message.NumeroContaCorrenteOrigem);
            var contaDestino = await _informacoesContaService.ObterNumeroContaPorNumero(message.NumneroContaCorrenteDestino);
            #endregion
            return null;
        }
    }
}
