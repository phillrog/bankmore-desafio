using BankMore.Infra.Apis.Producers;

using KafkaFlow;
using KafkaFlow.Serializer;

namespace BankMore.Services.Api.Identidade.Configurations;
    
public static class KafkaSetup
{
    public static void AddKafkaSetup(this IServiceCollection services, IConfiguration configuration)
    {
        const string topicName = "usuario-criado";
        var broker =  configuration.GetValue<string>("Kafka:Endereco");
        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[] { broker })
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
