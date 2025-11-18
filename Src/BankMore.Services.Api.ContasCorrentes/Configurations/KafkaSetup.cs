using BankMore.Infra.Kafka.Consumers;
using BankMore.Infra.Kafka.Managers;
using BankMore.Infra.Kafka.Producers;
using BankMore.Infra.Kafka.Responses;
using BankMore.Infra.Kafka.Services;
using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Serializer;

namespace BankMore.Services.Api.ContasCorrentes.Configurations;

public static class KafkaSetup
{
    public static void AddKafkaSetup(this IServiceCollection services, IConfiguration configuration)
    {

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
                .AddProducer<ICadastroContaRequestProducer>(
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
                .AddProducer<IInforcacoesContaResponseProducer>(
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
                .AddProducer<IMovimentacaoRequestProducer>(
                    producer => producer
                        .DefaultTopic("movimentar.conta.requisicao")
                        .WithAcks(KafkaFlow.Acks.All)
                        .WithProducerConfig(new ProducerConfig
                        {
                            EnableIdempotence = true,
                            MessageTimeoutMs = 10000,
                        })
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())

                ).AddProducer<IMovimentacaoResponseProducer>(
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
                .AddProducer<ISaldoRequestProducer>(
                    producer => producer
                        .DefaultTopic("buscar-saldo.conta.requisicao")
                        .WithAcks(KafkaFlow.Acks.All)
                        .WithProducerConfig(new ProducerConfig
                        {
                            EnableIdempotence = true,
                            MessageTimeoutMs = 10000,
                        })
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())

                ).AddProducer<ISaldoResponseProducer>(
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
