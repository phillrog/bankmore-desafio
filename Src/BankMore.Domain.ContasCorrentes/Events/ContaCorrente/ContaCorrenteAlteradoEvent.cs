using BankMore.Domain.Core.Events;

namespace BankMore.Domain.ContasCorrentes.Events;

public class ContaCorrenteAlteradoEvent : Event
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public int Numero { get; private set; }
    public string Cpf { get; private set; }
    public string Senha { get; private set; }
    public string Salt { get; private set; }
    public bool Ativo { get; private set; }

    public ContaCorrenteAlteradoEvent(Guid id, string nome, int numero, string cpf, string senha, bool ativo)
    {
        Id = id;
        Nome = nome;
        Numero = numero;
        Cpf = cpf;
        Senha = senha;
        Ativo = ativo;
    }
}
