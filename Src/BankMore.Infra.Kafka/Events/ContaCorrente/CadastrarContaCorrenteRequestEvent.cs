using System.Runtime.Serialization;

namespace BankMore.Infra.Kafka.Events.ContaCorrente
{

    [DataContract]
    public class CadastrarContaCorrenteRequestEvent
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }
        [DataMember(Order = 2)]
        public string Nome { get; set; }
        [DataMember(Order = 3)]
        public string Cpf { get; set; }
        [DataMember(Order = 4)]
        public string Senha { get; set; }
        [DataMember(Order = 5)]
        public Guid CorrelationId { get; set; }

        [DataMember(Order = 6)]
        public string ReplyTopic { get; set; }

        public CadastrarContaCorrenteRequestEvent()
        {

        }

        public CadastrarContaCorrenteRequestEvent(Guid id, string nome, string cpf, string senha, Guid correlationId, string replyTopic)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
            Senha = senha;
            CorrelationId = correlationId;
            ReplyTopic = replyTopic;
        }
    }
}