using BankMore.Application.Common.Commands;
using BankMore.Application.Common.Topicos.Saga;
using BankMore.Domain.Common.Enums;
using BankMore.Domain.Common.Events;
using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.ContasCorrentes.Models;
using MediatR;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace BankMore.Application.ContasCorrentes.Commands
{
    public class TentarCreditarContaCommandHandler : IRequestHandler<TentarCreditoCommand, MovimentacaoContaRespostaEvent>
    {
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public TentarCreditarContaCommandHandler(IIdempotenciaRepository idempotenciaRepository,
            IMovimentoRepository movimentoRepository)
        {
            _idempotenciaRepository = idempotenciaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<MovimentacaoContaRespostaEvent> Handle(TentarCreditoCommand command, CancellationToken cancellationToken)
        {

            try
            {
                var novoId = Guid.NewGuid();
                var movimento = new Movimento(novoId, command.IdContaCorrenteDestino, command.DataMovimento, 'C', command.Valor);
                movimento.DefinirIdTransferencia(novoId);
                _movimentoRepository.Add(movimento);
                _movimentoRepository.SaveChanges();

                var requisicao = ParseJson(command);
                var resultado = ParseJson(movimento);

                var idempotencia = new Idempotencia(novoId, command.IdContaCorrenteDestino, requisicao, resultado);
                idempotencia.DefinirIdTransferencia(novoId);
                _idempotenciaRepository.Add(idempotencia);
                _idempotenciaRepository.SaveChanges();

                return new MovimentacaoContaRespostaEvent
                {
                    Id = novoId,
                    Status = (int)StatusEnum.CREDITADO,
                    TipoMovimentacao = "C",
                    IdContaCorrenteDestino = command.IdContaCorrenteDestino,
                    CorrelationId = command.CorrelationId,
                    Valor = command.Valor,
                    DataMovimento = command.DataMovimento,
                    IdContaCorrenteOrigem = command.IdContaCorrenteOrigem,
                    Topico = SagaTopico.DebitarConta,
                    IsCompensation = false
                };
            }
            catch (Exception ex)
            {
                /// ATUALIZAR NO OUTBOX
                return new MovimentacaoContaRespostaEvent
                {
                    Id = command.Id,
                    Status = (int)StatusEnum.ERRO_CREDITO,
                    TipoMovimentacao = "C",
                    IdContaCorrenteDestino = command.IdContaCorrenteDestino,
                    CorrelationId = command.CorrelationId,
                    Valor = command.Valor,
                    DataMovimento = command.DataMovimento,
                    IdContaCorrenteOrigem = command.IdContaCorrenteOrigem,
                    Topico = SagaTopico.DebitarConta,
                    ErrorMessage = ex.Message,
                    IsCompensation = true
                };
            }
        }

        public string ParseJson(object obj)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            return JsonSerializer.Serialize(obj, options);
        }
    }

}

