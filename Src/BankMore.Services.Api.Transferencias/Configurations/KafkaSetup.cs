using BankMore.Infra.Kafka.Consumers;
using BankMore.Infra.Kafka.Producers;
using BankMore.Infra.Kafka.Responses;
using BankMore.Infra.Kafka.Services;
using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Serializer;

namespace BankMore.Services.Api.Transferencias;

public static class KafkaSetup
{
    public static void AddKafkaSetup(this IServiceCollection services, IConfiguration configuration)
    {

        var broker = configuration.GetValue<string>("Kafka:Endereco");
        services.AddKafka(kafka => kafka
            .UseConsoleLog()
            .AddCluster(cluster => cluster
                .WithBrokers(new[] { broker })                
                .CreateTopicIfNotExists("informacoes.conta.requisicao", 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic("informacoes.conta.requisicao")
                    .WithGroupId("grupo.aplicacao.transferencia")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares

                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<InformacoesContaRequestConsumer>())
                    )
                )
                .AddProducer<IInforcacoesContaRequestProducer>(
                    producer => producer
                        .DefaultTopic("informacoes.conta.requisicao")
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())

                )
                .CreateTopicIfNotExists("informacoes.conta.resposta", 1, 1)
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
                .AddConsumer(consumer => consumer
                    .Topic("informacoes.conta.resposta")
                    .WithGroupId("grupo.aplicacao.transferencia")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(handlers => handlers.AddHandler<NumeroContaReplyManager>())
                    )
                )
            )
        );

        // Service
        services.AddScoped<InformacoesContaService>();
    }
}
