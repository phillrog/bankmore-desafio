using BankMore.Application.Common.Topicos.Saga;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Domain.Common.Interfaces;
using BankMore.Infra.Kafka.Consumers;
using BankMore.Infra.Kafka.Managers;
using BankMore.Infra.Kafka.Producers;
using BankMore.Infra.Kafka.Responses;
using BankMore.Infra.Kafka.Services;
using BankMore.Infra.Kafka.Tags;
using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Serializer;


namespace BankMore.Services.Api.ContasCorrentes.Configurations;

public static class KafkaSetup
{
    public static void AddKafkaSetup(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMovimentacaoRespostaProducer, MovimentacaoRespostaProducer>();
        services.AddSingleton<IDebitarContaProducerDispatcher, DebitarProducerDispatcher>();
        services.AddSingleton<IValidarDebitoProducerDispatcher, ValidarDebitoProducerDispatcher>();
        services.AddSingleton<ICreditarContaProducerDispatcher, CreditarContaProducerDispatcher>();
        services.AddSingleton<IEstornoDebitoContaProducerDispatch, EstornoDebitoContaProducerDispatch>();
        services.AddScoped<ISaldoService, SaldoService>();
        services.AddScoped<IExtratoService, ExtratoService>();
        const string topicName = "usuario-criado";
        var broker = configuration.GetValue<string>("Kafka:Endereco");
        services.AddKafka(kafka => kafka
            .UseConsoleLog()
            .AddCluster(cluster => cluster
                .WithBrokers(new[] { broker })
                .CreateTopicIfNotExists(topicName, 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic(topicName)
                    .WithGroupId("grupo.aplicacao.contacorrente")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares

                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<UsuarioCriadoConsumerHandler>())
                    )
                )
                .CreateTopicIfNotExists("informacoes.conta.requisicao", 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic("informacoes.conta.requisicao")
                    .WithGroupId("grupo.aplicacao.contacorrente")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares

                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<InformacoesContaRequestConsumer>())
                    )
                )
                .CreateTopicIfNotExists("cadastrar.conta.requisicao", 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic("cadastrar.conta.requisicao")
                    .WithGroupId("grupo.aplicacao.contacorrente")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares

                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<CadastrarContaRequestConsumer>())
                    )
                )
                .CreateTopicIfNotExists("movimentar.conta.requisicao", 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic("movimentar.conta.requisicao")
                    .WithGroupId("grupo.aplicacao.contacorrente")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares

                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<MovimentacaoRequestConsumer>())
                    )
                )
                .CreateTopicIfNotExists("movimentar.conta.resposta", 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic("movimentar.conta.resposta")
                    .WithGroupId("grupo.aplicacao.contacorrente")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(handlers => handlers.AddHandler<MovimentacaoReplyManager>())
                    )
                )
                .AddProducer<ICadastroContaRequestProducerTag>(
                    producer => producer
                        .DefaultTopic("cadastrar.conta.requisicao")
                        .WithAcks(KafkaFlow.Acks.All)
                        .WithProducerConfig(new ProducerConfig
                        {
                            EnableIdempotence = true,
                            MessageTimeoutMs = 10000,
                        })
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())

                )
                .AddProducer<IInforcacoesContaResponseProducerTag>(
                    producer => producer
                        .DefaultTopic("informacoes.conta.resposta")
                        .WithAcks(KafkaFlow.Acks.All)
                        .WithProducerConfig(new ProducerConfig
                        {
                            EnableIdempotence = true,
                            MessageTimeoutMs = 10000,
                        })
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())
                )
                .AddProducer<IMovimentacaoRequestProducerTag>(
                    producer => producer
                        .DefaultTopic("movimentar.conta.requisicao")
                        .WithAcks(KafkaFlow.Acks.All)
                        .WithProducerConfig(new ProducerConfig
                        {
                            EnableIdempotence = true,
                            MessageTimeoutMs = 10000,
                        })
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())

                ).AddProducer<IMovimentacaoResponseProducerTag>(
                    producer => producer
                        .DefaultTopic("movimentar.conta.resposta")
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
                    .WithGroupId("grupo.aplicacao.contacorrente")
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
                    .WithGroupId("grupo.aplicacao.contacorrente")
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

                ////---------------- \\\
                /// --- SAGA ------- \\\
                /// ---------------- \\\
                .CreateTopicIfNotExists(SagaTopico.DebitarConta, 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic(SagaTopico.DebitarConta)
                    .WithGroupId(SagaTopico.ContaCorrenteGrupo)
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<JsonCoreDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<DebitarContaConsumer>()
                        )
                    )
                )
                .AddProducer<IValidarDebitoProducerTag>(
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
                .CreateTopicIfNotExists(SagaTopico.CreditarConta, 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic(SagaTopico.CreditarConta)
                    .WithGroupId(SagaTopico.ContaCorrenteGrupo)
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<JsonCoreDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<TentarCreditoContaConsumer>())
                    )
                )
                .CreateTopicIfNotExists(SagaTopico.EstornarConta, 1, 1)
                .AddConsumer(consumer => consumer
                    .Topics(SagaTopico.EstornarConta)
                    .WithGroupId(SagaTopico.ContaCorrenteGrupo)
                    .WithBufferSize(100)
                    .WithWorkersCount(5)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<JsonCoreDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<EstornoDebitoConsumer>())
                    )
                )

            )
        );

        /// Movimentações
        services.AddSingleton<MovimentacaoReplyManager>();
        services.AddScoped<MovimentarContaService>();

        /// Saldo
        services.AddSingleton<SaldoReplyManager>();
        services.AddScoped<SaldoService>();


    }
}
