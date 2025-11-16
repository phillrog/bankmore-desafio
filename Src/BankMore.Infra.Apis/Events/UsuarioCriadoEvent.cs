namespace BankMore.Infra.Apis.Events;

public class UsuarioCriadoEvent
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string Senha { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
