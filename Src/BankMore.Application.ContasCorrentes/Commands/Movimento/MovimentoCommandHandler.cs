using AutoMapper;
using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Application.Idempotencia.Services;
using BankMore.Domain.Common.CommandHandlers;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Enums;
using BankMore.Domain.ContasCorrentes.Events;
using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.ContasCorrentes.Interfaces.Services;
using BankMore.Domain.ContasCorrentes.Models;
using BankMore.Domain.ContasCorrentes.Services;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Events;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Core.Notifications;
using BankMore.Infra.Data.ContasCorrentes.Repository;
using MediatR;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Transactions;


namespace BankMore.Application.ContasCorrentes.Commands;

public class MovimentoCommandHandler : CommandHandler,
    IRequestHandler<CadastrarNovaMovimentacaoCommand, Result<MovimentacaoRelaizadaDto>>
{
    #region [ SERVICES ]

    private readonly IMediatorHandler _bus;
    private readonly IMovimentoRepository _movimentoRepository;
    private readonly IContaCorrenteService _contaCorrenteService;
    private readonly IUser _user;
    private readonly IMapper _mapper;
    private readonly IIdempotenciaService _idempotenciaService;
    private readonly ICorrentistaService _correntistaService;
    #endregion

    #region [ CONSTRUTOR ]

    public MovimentoCommandHandler(
        IUnitOfWork uow,
        IMediatorHandler bus,
        INotificationHandler<DomainNotification> notifications,
        IMovimentoRepository movimentoRepository,
        IContaCorrenteService contaCorrenteService,
        IIdempotenciaService idempotenciaService,
        IUser user,
        IMapper mapper,
        ICorrentistaService correntistaService)
        : base(uow, bus, notifications)
    {
        _bus = bus;
        _movimentoRepository = movimentoRepository;
        _contaCorrenteService = contaCorrenteService;
        _user = user;
        _mapper = mapper;
        _idempotenciaService = idempotenciaService;
        _correntistaService = correntistaService;
    }
    #endregion

    #region [ HANDLERS ]       

    public async Task<Result<MovimentacaoRelaizadaDto>> Handle(CadastrarNovaMovimentacaoCommand message, CancellationToken cancellationToken)
    {
        #region [ VALIDAÇÕES]
        var numeroConta = _user.Conta is not null ? Convert.ToInt32(_user.Conta) : message.NumeroConta;
        if (!message.IsValid())
        {
            NotifyValidationErrors(message);
            return Result<MovimentacaoRelaizadaDto>.Failure(message, Erro.INVALID_DOCUMENT);
        }

        if (await _movimentoRepository.GetByExpressionAsync(d => d.Id == message.Id) != null)
        {
            var erro = "Movimentação já cadastrada.";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Result<MovimentacaoRelaizadaDto>.Failure(erro, Erro.INVALID_DOCUMENT);
        }

        if (message.Valor <= 0)
        {
            var erro = "Apenas valores positivos podem ser recebidos";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Result<MovimentacaoRelaizadaDto>.Failure(erro, Erro.INVALID_VALUE);
        }

        if (message.Valor <= 0)
        {
            var erro = "Apenas valores positivos podem ser recebidos";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Result<MovimentacaoRelaizadaDto>.Failure(erro, Erro.INVALID_VALUE);
        }

        var conta = await _correntistaService.BuscarConta(numeroConta);

        if (conta is null)
        {
            var erro = "Apenas contas correntes cadastradas podem receber movimentação";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Result<MovimentacaoRelaizadaDto>.Failure(erro, Erro.INVALID_ACCOUNT);
        }

        if (conta.Inativa())
        {
            var erro = "Apenas contas correntes ativas podem receber movimentação.";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Result<MovimentacaoRelaizadaDto>.Failure(erro, Erro.INACTIVE_ACCOUNT);
        }

        if (message.EhDebito() && conta.Numero != numeroConta)
        {
            var erro = "Apenas o tipo “crédito” pode ser aceito caso o número da conta seja diferente do usuário logado";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Result<MovimentacaoRelaizadaDto>.Failure(erro, Erro.INVALID_TYPE);
        }

        if (await _idempotenciaService.Existe(message.Id))
        {
            var erro = $"Movimentação ID [{message.Id}] já foi efetuada!";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Result<MovimentacaoRelaizadaDto>.Failure(erro, Erro.INVALID_TYPE);
        }

        var saldoDetalhado = await _correntistaService.Saldo(numeroConta);

        if (message.EhDebito() && saldoDetalhado.SaldoInsuficiente(message.Valor))
        {
            var erro = "Saldo insuficiente para esta movimentação.";
            _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
            return Result<MovimentacaoRelaizadaDto>.Failure(erro, Erro.INSUFFICIENT_FUNDS);
        }

        #endregion

        #region [ TRANSAÇÃO]

        await BeginTransactionAsync();
        try
        {
            message.AtualizarIdContaCorrente(conta.Id);
            var movimento = _mapper.Map<Movimento>(message);

            _movimentoRepository.Add(movimento);
            _movimentoRepository.SaveChanges();

            decimal novoSaldo;

            if (message.TipoMovimento == TipoMovimento.C)
            {
                novoSaldo = saldoDetalhado.SaldoAtualizado + movimento.Valor;
            }
            else
            {
                novoSaldo = saldoDetalhado.SaldoAtualizado - movimento.Valor;
            }

            var retorno = new MovimentacaoRelaizadaDto
            {
                Id = movimento.Id,
                NumeroConta = conta.Numero,
                DataHora = movimento.DataMovimento,
                Tipo = movimento.TipoMovimento,
                Valor = movimento.Valor,
                SaldoAposMovimentacao = novoSaldo
            };

            var requisicao = ParseJson(message);
            var resultado = ParseJson(retorno);

            var retornoIdempotencia = await _idempotenciaService.Cadastrar(new IdempotenciaViewModel(
                movimento.Id,
                movimento.IdContaCorrente,
                requisicao,
                resultado
             ));

            if (!retornoIdempotencia.IsSuccess)
            {
                var erro = "Falha ao gravar idempotência.";
                _bus.RaiseEvent(new DomainNotification(message.MessageType, erro));
                return Result<MovimentacaoRelaizadaDto>.Failure(erro, Erro.INERNAL_ERROR);
            }

            if (Commit())
            {

                _bus.RaiseEvent(new MovimentoRegistradoEvent(
                    movimento.Id,
                    movimento.IdContaCorrente,
                    movimento.DataMovimento,
                    movimento.TipoMovimento,
                    movimento.Valor));

                return Result<MovimentacaoRelaizadaDto>.Success(retorno);
            }

            return Result<MovimentacaoRelaizadaDto>.Failure("Ops! Algo deu errado ao salvar movimento", Erro.INERNAL_ERROR);
        }
        catch (Exception)
        {
            RollbackTransaction();
            throw;
        }


        #endregion
    }

    #endregion

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
