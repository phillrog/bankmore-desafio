using System.Runtime.Serialization;

namespace BankMore.Infra.Kafka.Events;

[DataContract]
public class UsuarioCriadoEvent
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
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public UsuarioCriadoEvent()
    {

    }

    public UsuarioCriadoEvent(Guid id, string nome, string cpf, string senha)
    {
        Id = id;
        Nome = nome;
        Cpf = cpf;
        Senha = senha;
    }
}
