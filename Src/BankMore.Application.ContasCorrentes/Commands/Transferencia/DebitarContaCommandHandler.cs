using BankMore.Application.Common.Commands;
using BankMore.Application.Common.Topicos.Saga;
using BankMore.Domain.Common.Enums;
using BankMore.Domain.Common.Events;
using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.ContasCorrentes.Models;
using MediatR;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankMore.Application.ContasCorrentes.Commands.Transferencia
{
    public class DebitarContaCommandHandler : IRequestHandler<DebitarContaCommand, MovimentacaoContaRespostaEvent>
    {
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public DebitarContaCommandHandler(IIdempotenciaRepository idempotenciaRepository,
            IMovimentoRepository movimentoRepository)
        {
            _idempotenciaRepository = idempotenciaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<MovimentacaoContaRespostaEvent> Handle(DebitarContaCommand command, CancellationToken cancellationToken)
        {

            try
            {
                var novoId = Guid.NewGuid();
                var movimento = new Movimento(novoId, command.IdContaCorrenteOrigem, command.DataMovimento, 'D', command.Valor);
                movimento.DefinirIdTransferencia(command.Id);

                _movimentoRepository.Add(movimento);
                _movimentoRepository.SaveChanges();

                var requisicao = ParseJson(command);
                var resultado = ParseJson(movimento);

                var idempotencia = new Idempotencia(novoId, command.IdContaCorrenteDestino, requisicao, resultado);
                idempotencia.DefinirIdTransferencia(command.Id);
                _idempotenciaRepository.Add(idempotencia);
                _idempotenciaRepository.SaveChanges();

                return new MovimentacaoContaRespostaEvent
                {
                    Id = command.Id,
                    Status = (int)StatusEnum.DEBITADO,
                    TipoMovimentacao = "D",
                    IdContaCorrenteDestino = command.IdContaCorrenteDestino,
                    CorrelationId = command.CorrelationId,
                    Valor = command.Valor,
                    DataMovimento = command.DataMovimento,
                    IdContaCorrenteOrigem = command.IdContaCorrenteOrigem,
                    Topico = SagaTopico.DebitarConta
                };
            }
            catch (Exception ex)
            {
                /// ATUALIZAR NO OUTBOX
                return new MovimentacaoContaRespostaEvent
                {
                    Id = command.Id,
                    Status = (int)StatusEnum.ERRO_DEBITO,
                    TipoMovimentacao = "D",
                    IdContaCorrenteDestino = command.IdContaCorrenteDestino,
                    CorrelationId = command.CorrelationId,
                    Valor = command.Valor,
                    DataMovimento = command.DataMovimento,
                    IdContaCorrenteOrigem = command.IdContaCorrenteOrigem,
                    Topico = SagaTopico.DebitarConta,
                    ErrorMessage = ex.Message,
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
