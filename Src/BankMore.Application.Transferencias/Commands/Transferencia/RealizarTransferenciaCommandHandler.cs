using AutoMapper;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Application.Transferencias.ViewModels;
using BankMore.Domain.Common.CommandHandlers;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Enums;
using BankMore.Domain.ContasCorrentes.Events;
using BankMore.Domain.ContasCorrentes.Models;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Core.Notifications;
using BankMore.Domain.Transferencias.Dtos;
using BankMore.Domain.Transferencias.Interfaces;
using BankMore.Domain.Transferencias.Models;
using BankMore.Infra.Data.Common.Repository;
using BankMore.Infra.Kafka.Services;
using MediatR;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankMore.Application.Transferencias.Commands
{
    public class RealizarTransferenciaCommandHandler : CommandHandler,
        IRequestHandler<RealizarTransferenciaCommand, Result<TransferenciaDto>>
    {
        #region [ SERVICES ]

        private readonly IMediatorHandler _bus;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private readonly IIdempotenciaService _idempotenciaService;
        private readonly IInformacoesContaRespository _informacoesContaRespository;
        private readonly ITransferenciaRepository _transferenciaRepository;

        #endregion

        #region [ CONSTRUTOR ]

        public RealizarTransferenciaCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,            
            IUser user,
            IMapper mapper,
            IIdempotenciaService idempotenciaService,
            IInformacoesContaRespository informacoesContaRespository,
            ITransferenciaRepository transferenciaRepository)
            : base(uow, bus, notifications)
        {
            _bus = bus;
            _user = user;
            _mapper = mapper;
            _idempotenciaService = idempotenciaService;
            _informacoesContaRespository = informacoesContaRespository;
            _transferenciaRepository = transferenciaRepository;
        }

        #endregion
        public async Task<Result<TransferenciaDto>> Handle(RealizarTransferenciaCommand message, CancellationToken cancellationToken)
        {
            #region [ VALIDAÇÕES ]
            if (!message.IsValid())
            {
                NotifyValidationErrors(message);
                return Result<TransferenciaDto>.Failure(message, Erro.INVALID_DOCUMENT);
            }

            if (message.Valor <= 0)
            {
                var erro = "Apenas valores positivos podem ser recebidos. Por favor verifique o valor.";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INVALID_VALUE);
            }

            var contaOrigem = await _informacoesContaRespository.GetByNumero(message.NumeroContaCorrenteOrigem);

            if (contaOrigem is null || contaOrigem.Id == Guid.Empty)
            {
                var erro = $"Conta corrente de origem {message.NumeroContaCorrenteOrigem} inválida.";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INVALID_ACCOUNT);
            }

            if (!contaOrigem.Ativo)
            {
                var erro = "Apenas contas correntes ativas podem realizar transferências.Por favor verifique a conta origem.";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INVALID_ACCOUNT);
            }

            var contaDestino = await _informacoesContaRespository.GetByNumero(message.NumneroContaCorrenteDestino);

            if (contaDestino is null || contaDestino.Id == Guid.Empty)
            {
                var erro = $"Conta corrente de destino {message.NumeroContaCorrenteOrigem} inválida.";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INVALID_ACCOUNT);
            }

            if (!contaDestino.Ativo)
            {
                var erro = "Apenas contas correntes ativas podem realizar transferências. Por favor verifique a conta destino.";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(message, Erro.INVALID_ACCOUNT);
            }

            if (await _idempotenciaService.Existe(message.Id))
            {
                var erro = $"Transferência com ID [{message.Id}] já foi efetuada!";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INVALID_TYPE);
            }

            #endregion

            await BeginTransactionAsync();
            try
            {
                var transferencia = _mapper.Map<Transferencia>(message);

                transferencia.AtualizarContaOrigem(contaOrigem.Id);
                transferencia.AtualizarContaDestino(contaDestino.Id);

                _transferenciaRepository.Add(transferencia);
                _transferenciaRepository.SaveChanges();

                var retorno = new TransferenciaDto
                {
                    Id = message.Id,
                    NumeroContaDestino = contaDestino.Numero,
                    Valor = message.Valor,
                };

                var requisicao = ParseJson(message);
                var resultado = ParseJson(retorno);

                var retornoIdempotencia = await _idempotenciaService.Cadastrar(new IdempotenciaViewModel(
                    transferencia.Id,
                    contaOrigem.Id,
                    requisicao,
                    resultado
                 ));

                if (!retornoIdempotencia.IsSuccess)
                {
                    var erro = "Falha ao gravar idempotência.";
                    _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                    return Result<TransferenciaDto>.Failure(erro, Erro.INTERNAL_ERROR);
                }

                if (Commit())
                {

                    return Result<TransferenciaDto>.Success(retorno);
                }

                return Result<TransferenciaDto>.Failure("Ops! Algo deu errado ao salvar transferência", Erro.INTERNAL_ERROR);
            }
            catch (Exception)
            {
                RollbackTransaction();
                var erro = "Ops! Algo deu errado ao salvar movimento. Por favor tente mais tarde!";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INTERNAL_ERROR);
            }
        }

        #region [ PRIVATE METODOS ]
        public string ParseJson(object obj)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            return JsonSerializer.Serialize(obj, options);
        }
        #endregion
    }
}
