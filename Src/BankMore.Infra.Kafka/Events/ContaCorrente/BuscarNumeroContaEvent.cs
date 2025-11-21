using System.Runtime.Serialization;

namespace BankMore.Infra.Kafka.Events;

[DataContract]
public class BuscarNumeroContaEvent
{
    [DataMember(Order = 1)]
    public string Cpf { get; set; }
    [DataMember(Order = 2)]
    public Guid CorrelationId { get; set; }

    [DataMember(Order = 3)]
    public string ReplyTopic { get; set; }

    [DataMember(Order = 4)]
    public int Numero { get; set; }

    public BuscarNumeroContaEvent() { }

    public BuscarNumeroContaEvent(string cpf, Guid correlationId, string replyTopic)
    {
        Cpf = cpf;
        CorrelationId = correlationId;
        ReplyTopic = replyTopic;
    }

    public BuscarNumeroContaEvent(int numero, Guid correlationId, string replyTopic)
    {
        Numero = numero;
        CorrelationId = correlationId;
        ReplyTopic = replyTopic;
    }
}
