using BankMore.Application.ContasCorrentes.Consumers;
using KafkaFlow;
using KafkaFlow.Serializer;
using KafkaFlow.Configuration;

namespace BankMore.Services.Api.ContasCorrentes.Configurations;

public static class KafkaSetup
{
    public static void AddKafkaSetup(this IServiceCollection services)
    {

        const string topicName = "usuario-criado";

        services.AddKafka(kafka => kafka
            .UseConsoleLog()
            .AddCluster(cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .CreateTopicIfNotExists(topicName, 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic(topicName)
                    .WithGroupId("grupo.aplicacao.contacorrente")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares

                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<UsuarioCriadoHandler>())
                    )
                )
            )
        );
    }
}
