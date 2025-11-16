using BankMore.Infra.Apis.Producers;

using KafkaFlow;
using KafkaFlow.Serializer;

namespace BankMore.Services.Api.Identidade.Configurations;
    
public static class KafkaSetup
{
    public static void AddKafkaSetup(this IServiceCollection services)
    {
        const string topicName = "usuario-criado";
        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[] { "localhost:9092" })
                        .CreateTopicIfNotExists(topicName, 1, 1)
                        .AddProducer<UsuarioCriadoProducer>(
                            producer => producer
                                .DefaultTopic(topicName)
                                .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())

                        )
                )
        );
       
    }
}
