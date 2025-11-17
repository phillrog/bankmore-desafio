using BankMore.Infra.Kafka.Consumers;
using BankMore.Infra.Kafka.Producers;
using BankMore.Infra.Kafka.Responses;
using BankMore.Infra.Kafka.Services;
using KafkaFlow;
using KafkaFlow.Configuration;
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
                        .AddTypedHandlers(h => h.AddHandler<InformacoesContaConsumerHandler>())
                    )
                )
                .AddConsumer(consumer => consumer
                    .Topic("cadastrar.conta.requisicao")
                    .WithGroupId("grupo.aplicacao.contacorrente")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares

                        .AddDeserializer<ProtobufNetDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<CadastrarContaConsumerHandler>())
                    )
                )
                .AddProducer<ICadastroContaRequestProducer>(
                    producer => producer
                        .DefaultTopic("cadastrar.conta.requisicao")
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())

                )
                .AddProducer<IInforcacoesContaResponseProducer>(
                    producer => producer
                        .DefaultTopic("informacoes.conta.resposta")
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())
                )
            )
        );

        /// ---- responses
        /// Tem que ser singleton se não o TryRemove não encontra a chave da mensagem
        //services.AddSingleton<NumeroContaResponseManager>();       

        ///// ---- serviços
        //services.AddScoped<ContaCorrenteService>();
    }
}
