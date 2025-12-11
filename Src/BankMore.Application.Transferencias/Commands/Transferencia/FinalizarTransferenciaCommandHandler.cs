using BankMore.Application.Common.Topicos.Saga;
using BankMore.Domain.Common.Enums;
using BankMore.Domain.Common.Events;
using BankMore.Domain.Transferencias.Interfaces;
using MediatR;


namespace BankMore.Application.Transferencias.Commands
{
    public class FinalizarTransferenciaCommandHandler : IRequestHandler<FinalizarTransferenciaCommand, MovimentacaoContaRespostaEvent>
    {
        private readonly ITransferenciaRepository _transferenciaRepository;

        public FinalizarTransferenciaCommandHandler(ITransferenciaRepository repository)
        {
            _transferenciaRepository = repository;
        }

        public async Task<MovimentacaoContaRespostaEvent> Handle(FinalizarTransferenciaCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var transferencia = await _transferenciaRepository.GetByExpressionAsync(t => t.Id == command.Id);

                if (transferencia is null) return new MovimentacaoContaRespostaEvent();

                transferencia.AtualizarStatus((StatusEnum)command.Status);
                transferencia.AtualizarDataUltimaAlteracao(DateTime.Now);

                await _transferenciaRepository.AtualizarStatusAsync(transferencia);

                return new MovimentacaoContaRespostaEvent
                {
                    Id = command.Id,
                    Status = command.Status,
                    IdContaCorrenteDestino = command.IdContaCorrenteDestino,
                    CorrelationId = command.CorrelationId,
                    Valor = command.Valor,
                    DataMovimento = command.DataMovimento,
                    IdContaCorrenteOrigem = command.IdContaCorrenteOrigem,
                    Topico = SagaTopico.FinalizadaTransferencia,
                };
                ///FIM
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
