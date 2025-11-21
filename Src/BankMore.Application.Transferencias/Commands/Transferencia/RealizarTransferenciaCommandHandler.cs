using AutoMapper;
using BankMore.Application.Common.Querys.ContaCorrente;
using BankMore.Application.Common.Topicos.Saga;
using BankMore.Domain.Common.CommandHandlers;
using BankMore.Domain.Common.Dtos;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Core.Notifications;
using BankMore.Domain.Transferencias.Dtos;
using BankMore.Domain.Transferencias.Events;
using BankMore.Domain.Transferencias.Interfaces;
using BankMore.Domain.Transferencias.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace BankMore.Application.Transferencias.Commands
{
    public class RealizarTransferenciaCommandHandler : CommandHandler,
        IRequestHandler<RealizarTransferenciaCommand, Result<TransferenciaDto>>
    {
        #region [ PRIVATE VARIÁVEIS ]
        private const string ConnectionStringName = "DefaultConnection";
        #endregion
        #region [ SERVICES ]

        private readonly IMediatorHandler _bus;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly IInformacoesContaRespository _informacoesContaRespository;
        private readonly ITransferenciaRepository _transferenciaRepository;
        private readonly IOutboxRepository _outboxRepository;
        private readonly IConfiguration _configuration;
        private readonly ISaldoService _saldoService;

        #endregion

        #region [ CONSTRUTOR ]

        public RealizarTransferenciaCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IUser user,
            IMapper mapper,
            IIdempotenciaRepository idempotenciaRepository,
            IInformacoesContaRespository informacoesContaRespository,
            ITransferenciaRepository transferenciaRepository,
            IOutboxRepository outboxRepository,
            IConfiguration configuration,
            ISaldoService saldoService
            )
            : base(uow, bus, notifications)
        {
            _bus = bus;
            _user = user;
            _mapper = mapper;
            _idempotenciaRepository = idempotenciaRepository;
            _informacoesContaRespository = informacoesContaRespository;
            _transferenciaRepository = transferenciaRepository;
            _outboxRepository = outboxRepository;
            _configuration = configuration;
            _saldoService = saldoService;
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
                var erro = $"Conta corrente de origem {message.NumeroContaCorrenteOrigem} é inválida.";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INVALID_ACCOUNT);
            }

            if (contaOrigem.Numero != Convert.ToUInt32(_user.Conta))
            {
                var erro = $"O número da conta origem {message.NumeroContaCorrenteOrigem} é inválida. Não pertence ao cliente.";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INVALID_ACCOUNT);
            }

            if (!contaOrigem.Ativo)
            {
                var erro = "Apenas contas correntes ativas podem realizar transferências.Por favor verifique a conta origem.";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INVALID_ACCOUNT);
            }

            var consultaSaldo = await _saldoService.ConsultarSaldo(Convert.ToInt32(_user.Conta));

            if (!consultaSaldo.IsSuccess)
            {
                var erro = string.Join(", ", consultaSaldo.Erros);
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INVALID_VALUE);
            }

            if (consultaSaldo.Data.SaldoAtualizado == 0)
            {
                var erro = "Saldo na conta insuficiente";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INSUFFICIENT_FUNDS);
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

            if (_idempotenciaRepository.Exist(message.Id))
            {
                var erro = $"Transferência com ID [{message.Id}] já foi efetuada!";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<TransferenciaDto>.Failure(erro, Erro.INVALID_TYPE);
            }

            #endregion

            try
            {
                var transferencia = _mapper.Map<Transferencia>(message);
                transferencia.AtualizarContaOrigem(contaOrigem.Id);
                transferencia.AtualizarContaDestino(contaDestino.Id);

                var retorno = new TransferenciaDto
                {
                    Id = message.Id,
                    NumeroContaDestino = contaDestino.Numero,
                    Valor = message.Valor,
                };

                var requisicao = ParseJson(message);
                var resultado = ParseJson(retorno);

                _transferenciaRepository.Add(transferencia);

                var novaIdempotencia = new Idempotencia(transferencia.Id, contaOrigem.Id, requisicao, resultado);

                _idempotenciaRepository.Add(novaIdempotencia);

                var eventoIniciado = new TransferenciaIniciadaEvent(
                    transferencia.Id,
                    transferencia.Id,
                    transferencia.IdContaCorrenteOrigem,
                    transferencia.IdContaCorrenteDestino,
                    transferencia.Valor,
                    (int)transferencia.Status,
                    transferencia.DataMovimento
                );
                eventoIniciado.Topico = SagaTopico.IniciarTranferencia;

                var payload = ParseJson(retorno);
                _transferenciaRepository.SaveChanges();
                _outboxRepository.Add(eventoIniciado);

                var gravouRegistros = _transferenciaRepository.Exist(transferencia.Id)
                    && _idempotenciaRepository.Exist(transferencia.Id)
                    && await _outboxRepository.ExistTransfer(eventoIniciado.Id);

                if (!gravouRegistros)
                {
                    var erro = $"Ops! Falaha ao gravar dados já foi efetuada!";
                    _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                    return Result<TransferenciaDto>.Failure(erro, Erro.INVALID_TYPE);
                }
                return Result<TransferenciaDto>.Success(retorno);
            }
            catch (Exception ex)
            {
                var erro = "Ops! Falha crítica ao salvar transferência. Tente mais tarde!";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));

                return Result<TransferenciaDto>.Failure(erro, Erro.INTERNAL_ERROR);
            }
        }

        #region [ PRIVATE METODOS ]
        public string ParseJson(object obj)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Serialize(obj, options);
        }
        #endregion
    }
}
