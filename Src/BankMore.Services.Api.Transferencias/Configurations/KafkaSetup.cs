using BankMore.Application.Common.Topicos.Saga;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Transferencias.Interfaces;
using BankMore.Infra.Data.Common.Respository;
using BankMore.Infra.Data.Transferencias.Repository;
using BankMore.Infra.Kafka.Consumers;
using BankMore.Infra.Kafka.Consumers.Saga.Transferencia;
using BankMore.Infra.Kafka.Managers;
using BankMore.Infra.Kafka.Producers;
using BankMore.Infra.Kafka.Responses;
using BankMore.Infra.Kafka.Services;
using BankMore.Infra.Kafka.Tags;
using BankMore.Transferencias.Workers;
using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Serializer;

namespace BankMore.Services.Api.Transferencias;

public static class KafkaSetup
{
    public static void AddKafkaSetup(this IServiceCollection services, IConfiguration configuration)
    {
        var broker = configuration.GetValue<string>("Kafka:Endereco");

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = broker,
            Acks = Confluent.Kafka.Acks.All
        };

        // ------------------------------------------
        // 1. REGISTROS DE DEPENDÊNCIA (DI)
        // ------------------------------------------

        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<ITransferenciaRepository, TransferenciaRepository>(); // Adicionado para ITransf. Rep. no Scoped

        // Serviços da Saga
        services.AddSingleton<ISagaTransferenciaProducerDispatcher, SagaTransferenciaProducerDispatcher>();
        services.AddSingleton<IDebitarContaProducerDispatcher, DebitarProducerDispatcher>();
        services.AddSingleton<IValidarDebitoProducerDispatcher, ValidarDebitoProducerDispatcher>();
        services.AddSingleton<ICreditarContaProducerDispatcher, CreditarContaProducerDispatcher>();
        services.AddSingleton<IEstornoDebitoContaProducerDispatch, EstornoDebitoContaProducerDispatch>();
        services.AddSingleton<IFinalizarTransferenciaProducerDispatcher, FinalizarTransferenciaProducerDispatcher>();
        services.AddSingleton<SaldoReplyManager>();

        services.AddScoped<ISaldoService, SaldoService>();
        services.AddSingleton<IMovimentacaoRespostaProducer, MovimentacaoRespostaProducer>();
        // Handlers e Services (para MediatR e uso direto)
        services.AddScoped<InformacoesContaService>();

        // Logging e Configuração base do Kafka
        services.AddLogging(
            builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });
        services.AddSingleton(producerConfig);

        // ------------------------------------------
        // 2. CONFIGURAÇÃO DO KAFKAFLOW (Cluster e Topicos/Consumers/Producers)
        // ------------------------------------------
        services.AddKafka(kafka => kafka
            .UseConsoleLog()
            .AddCluster(cluster => cluster
                .WithBrokers(new[] { broker })

                // TOPIC: grupo.aplicacao.transferencia.requisicao
                .CreateTopicIfNotExists("grupo.aplicacao.transferencia.requisicao", 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic("grupo.aplicacao.transferencia.requisicao")
                    .WithGroupId("grupo.aplicacao.transferencia.requisicao")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<InformacoesContaRequestConsumer>())
                    )
                )
                .AddProducer<IInforcacoesContaRequestProducerTag>(
                    producer => producer
                        .DefaultTopic("grupo.aplicacao.transferencia.requisicao")
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())
                )

                // TOPIC: grupo.aplicacao.transferencia.resposta
                .CreateTopicIfNotExists("grupo.aplicacao.transferencia.resposta", 1, 1)
                .AddProducer<IInforcacoesContaResponseProducerTag>(
                    producer => producer
                        .DefaultTopic("grupo.aplicacao.transferencia.resposta")
                        .WithAcks(KafkaFlow.Acks.All)
                        .WithProducerConfig(new ProducerConfig
                        {
                            EnableIdempotence = true,
                            MessageTimeoutMs = 10000,
                        })
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())
                )

                .CreateTopicIfNotExists("buscar-saldo.conta.requisicao", 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic("buscar-saldo.conta.requisicao")
                    .WithGroupId("grupo.aplicacao.transferencia")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares

                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<SaldoRequestConsumer>())
                    )
                )
                .CreateTopicIfNotExists("buscar-saldo.conta.resposta", 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic("buscar-saldo.conta.resposta")
                    .WithGroupId("grupo.aplicacao.transferencia")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(handlers => handlers.AddHandler<SaldoReplyManager>())
                    )
                )
                .AddProducer<ISaldoRequestProducerTag>(
                    producer => producer
                        .DefaultTopic("buscar-saldo.conta.requisicao")
                        .WithAcks(KafkaFlow.Acks.All)
                        .WithProducerConfig(new ProducerConfig
                        {
                            EnableIdempotence = true,
                            MessageTimeoutMs = 10000,
                        })
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())

                ).AddProducer<ISaldoResponseProducerTag>(
                    producer => producer
                        .DefaultTopic("buscar-saldo.conta.resposta")
                        .WithAcks(KafkaFlow.Acks.All)
                        .WithProducerConfig(new ProducerConfig
                        {
                            EnableIdempotence = true,
                            MessageTimeoutMs = 10000,
                        })
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())
                )
                .AddConsumer(consumer => consumer
                    .Topic("grupo.aplicacao.transferencia.requisicao")
                    .WithGroupId("grupo.aplicacao.transferencia.requisicao.reply") 
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(handlers => handlers.AddHandler<NumeroContaReplyManager>())
                    )
                )

                .AddProducer<ISagaProducerTag>(
                    producer => producer
                        .DefaultTopic(SagaTopico.IniciarTranferencia)
                        .WithAcks(KafkaFlow.Acks.All)
                        .WithProducerConfig(new ProducerConfig
                        {
                            EnableIdempotence = true,
                            MessageTimeoutMs = 10000,
                        })
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                )

                .CreateTopicIfNotExists(SagaTopico.ValidarDebitoConta, 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic(SagaTopico.ValidarDebitoConta)
                    .WithBufferSize(100)
                    .WithGroupId(SagaTopico.TranferenciaGrupo)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<JsonCoreDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<ValidarDebitoConsumer>())
                    )
                )


                .AddConsumer(consumer => consumer
                    .Topic(SagaTopico.ValidarDebitoConta)
                    .WithGroupId(SagaTopico.TranferenciaGrupo)
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<JsonCoreDeserializer>() // Use o Deserializer correto para o DTO
                        .AddTypedHandlers(handlers =>
                            handlers.AddHandler<ValidarDebitoConsumer>()
                        )
                    )
                )
                .AddProducer<IValidarDebitoProducerTag>(
                    producer => producer
                        .DefaultTopic(SagaTopico.DebitarConta)
                        .WithAcks(KafkaFlow.Acks.All)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())

                )
               .CreateTopicIfNotExists(SagaTopico.IniciarTranferencia, 1, 1)
               .AddConsumer(consumer => consumer
                    .Topics(SagaTopico.IniciarTranferencia)
                    .WithGroupId(SagaTopico.TranferenciaGrupo)
                    .WithBufferSize(100)
                    .WithWorkersCount(5)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<JsonCoreDeserializer>() // Se o Outbox publica JSON
                        .AddTypedHandlers(h => h.AddHandler<TransferenciaIniciadaHandler>()) // Seu novo Handler!
                    )
                )
               .AddProducer<IDebitarContaProducerTag>(
                    producer => producer
                        .DefaultTopic(SagaTopico.DebitarConta)
                        .WithAcks(KafkaFlow.Acks.All)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())

                )
               .AddProducer<ICreditarContaProducerTag>(
                    producer => producer
                        .DefaultTopic(SagaTopico.CreditarConta)
                        .WithAcks(KafkaFlow.Acks.All)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                )

               .AddProducer<IEstornoDebitoContaTag>(
                    producer => producer
                        .DefaultTopic(SagaTopico.CreditarConta)
                        .WithAcks(KafkaFlow.Acks.All)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                )
               .CreateTopicIfNotExists(SagaTopico.FinalizadaTransferencia, 1, 1)
               .AddConsumer(consumer => consumer
                    .Topics(SagaTopico.FinalizadaTransferencia)
                    .WithGroupId(SagaTopico.TranferenciaGrupo)
                    .WithBufferSize(100)
                    .WithWorkersCount(5)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<JsonCoreDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<FinalizarTransferenciaConsumer>())
                    )
                )
               .AddProducer<IFinalizarTransferenciaProducerTag>(
                    producer => producer
                        .DefaultTopic(SagaTopico.FinalizadaTransferencia)
                        .WithAcks(KafkaFlow.Acks.All)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                )

            )
        );

        // Workers
        services.AddHostedService<OutboxMessageRelayer>();

    }
}