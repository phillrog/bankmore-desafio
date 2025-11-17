using BankMore.Infra.Kafka.Producers;
using BankMore.Infra.Kafka.Responses;
using BankMore.Infra.Kafka.Services;
using KafkaFlow;
using KafkaFlow.Serializer;

namespace BankMore.Services.Api.Identidade.Configurations;
    
public static class KafkaSetup
{
    public static void AddKafkaSetup(this IServiceCollection services, IConfiguration configuration)
    {
        //const string topicName = "usuario-criado";
        var broker =  configuration.GetValue<string>("Kafka:Endereco");
        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[] { broker })
                        //.CreateTopicIfNotExists(topicName, 1, 1)
                        //.AddProducer<UsuarioCriadoProducer>(
                        //    producer => producer
                        //        .DefaultTopic(topicName)
                        //        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())

                        //)
                        .CreateTopicIfNotExists("cadastrar.conta.requisicao", 1, 1)
                        .AddProducer<ICadastroContaRequestProducer>(
                            producer => producer
                                .DefaultTopic("cadastrar.conta.requisicao")
                                .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())

                        )
                        .CreateTopicIfNotExists("informacoes.conta.requisicao", 1, 1)
                        .AddProducer<IInforcacoesContaRequestProducer>(
                            producer => producer
                                .DefaultTopic("informacoes.conta.requisicao")
                                .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())

                        )
                        .CreateTopicIfNotExists("cadastrar.conta.resposta", 1, 1)
                        .AddConsumer(consumer => consumer
                            .Topic("cadastrar.conta.resposta")
                            .WithGroupId("grupo.aplicacao.contacorrente")
                            .WithBufferSize(100)
                            .WithWorkersCount(10)
                            .AddMiddlewares(middlewares => middlewares
                                .AddDeserializer<ProtobufNetDeserializer>()
                                .AddTypedHandlers(handlers => handlers.AddHandler<CadastroContaResponseManager>())
                            )
                        )
                        .CreateTopicIfNotExists("informacoes.conta.resposta", 1, 1)
                        .AddConsumer(consumer => consumer
                            .Topic("informacoes.conta.resposta")
                            .WithGroupId("grupo.aplicacao.contacorrente")
                            .WithBufferSize(100)
                            .WithWorkersCount(10)
                            .AddMiddlewares(middlewares => middlewares
                                .AddDeserializer<ProtobufNetDeserializer>()
                                .AddTypedHandlers(handlers => handlers.AddHandler<NumeroContaResponseManager>())
                            )
                        )
                )
        );


        /// ---- responses
        /// Tem que ser singleton se não o TryRemove não encontra a chave da mensagem
        services.AddSingleton<CadastroContaResponseManager>();
        /// ---- serviços
        services.AddScoped<InformacoesContaService>();
        services.AddScoped<ContaCorrenteService>();
    }
}
