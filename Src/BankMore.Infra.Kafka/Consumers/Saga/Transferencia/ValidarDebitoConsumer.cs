using BankMore.Application.Common.Commands;
using BankMore.Application.Common.Topicos.Saga;
using BankMore.Application.ContasCorrentes.Commands.Transferencia;
using BankMore.Application.Transferencias.Commands;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Domain.Common.Enums;
using BankMore.Domain.Common.Events;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Transferencias.Interfaces;
using BankMore.Infra.Kafka.Producers;
using KafkaFlow;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Infra.Kafka.Consumers
{
    public class ValidarDebitoConsumer : IMessageHandler<MovimentacaoContaRespostaEvent>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ValidarDebitoConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Handle(IMessageContext context, MovimentacaoContaRespostaEvent message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

                if (!await outboxRepository.ExistTransfer(message.CorrelationId)) return;

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                if (message.TipoMovimentacao == "D")
                {
                    if (message.Status == (int)StatusEnum.DEBITADO)
                    {
                        var dispatcher = scope.ServiceProvider.GetRequiredService<ICreditarContaProducerDispatcher>();
                        var command = new TentarCreditoCommand
                        {

                            Id = message.Id,
                            CorrelationId = message.CorrelationId,
                            DataMovimento = message.DataMovimento,
                            IdContaCorrenteOrigem = message.IdContaCorrenteOrigem,
                            IdContaCorrenteDestino = message.IdContaCorrenteDestino,
                            Valor = message.Valor,
                            Topico = SagaTopico.ValidarDebitoConta,
                            Descricao = message.Descricao
                        };

                        await dispatcher.PublishAsync(command, SagaTopico.CreditarConta);
                    }
                    else if (message.Status == (int)StatusEnum.ERRO_DEBITO)
                    {
                        // TODO: ATUALIZAR OUTBOX
                        await mediator.Send(new RegistrarFalhaTransferenciaCommand
                        {

                            Id = message.Id,
                            CorrelationId = message.CorrelationId,
                            DataMovimento = message.DataMovimento,
                            IdContaCorrenteOrigem = message.IdContaCorrenteOrigem,
                            IdContaCorrenteDestino = message.IdContaCorrenteDestino,
                            Valor = message.Valor,
                            Descricao = message.Descricao
                        });
                    }
                }
                else if (message.TipoMovimentacao == "C")
                {
                    if (message.Status == (int)StatusEnum.CREDITADO)
                    {
                        FinalizarTransferenciaCommand command;
                        var dispatcher = scope.ServiceProvider.GetRequiredService<IFinalizarTransferenciaProducerDispatcher>();
                        if (message.IsCompensation)
                        {
                            command = new FinalizarTransferenciaCommand
                            {
                                Id = message.Id,
                                CorrelationId = message.CorrelationId,
                                DataMovimento = message.DataMovimento,
                                IdContaCorrenteOrigem = message.IdContaCorrenteOrigem,
                                IdContaCorrenteDestino = message.IdContaCorrenteDestino,
                                Valor = message.Valor,
                                Status = (int)StatusEnum.COMPENSADA_COM_FALHA,
                                Descricao = message.Descricao
                            };
                        }
                        else
                        {
                            command = new FinalizarTransferenciaCommand
                            {
                                Id = message.Id,
                                CorrelationId = message.CorrelationId,
                                DataMovimento = message.DataMovimento,
                                IdContaCorrenteOrigem = message.IdContaCorrenteOrigem,
                                IdContaCorrenteDestino = message.IdContaCorrenteDestino,
                                Valor = message.Valor,
                                Status = (int)StatusEnum.CONCLUIDA,
                                Descricao = message.Descricao
                            };
                        }
                        await dispatcher.PublishAsync(command, SagaTopico.FinalizadaTransferencia);
                    }
                    else if (message.Status == (int)StatusEnum.ERRO_CREDITO)
                    {
                        await mediator.Send(new EstornarDebitoContaCommand
                        {
                            Id = message.Id,
                            CorrelationId = message.CorrelationId,
                            DataMovimento = message.DataMovimento,
                            IdContaCorrenteOrigem = message.IdContaCorrenteOrigem,
                            IdContaCorrenteDestino = message.IdContaCorrenteDestino,
                            Valor = message.Valor,
                            Descricao = message.Descricao
                        });
                    }
                }

                await AtualizarTransferenciaAsync(message, scope.ServiceProvider);                
            }
        }

        private async Task AtualizarTransferenciaAsync(MovimentacaoContaRespostaEvent message, IServiceProvider service)
        {
            var transferenciaRepository = service.GetRequiredService<ITransferenciaRepository>();
            var uOw = service.GetRequiredService<IUnitOfWork>();

            await uOw.BeginTransactionAsync();
            try
            {
                var transferencia = await transferenciaRepository.GetByExpressionAsync(t => t.Id == message.Id);

                if (transferencia is null) return;

                transferencia.AtualizarStatus((StatusEnum)message.Status);
                transferencia.AtualizarDataUltimaAlteracao(DateTime.UtcNow);
                transferenciaRepository.Update(transferencia);
                uOw.Commit();
            }
            catch (Exception)
            {
                uOw.RollbackTransaction();
                throw;
            }
        }
    }
}