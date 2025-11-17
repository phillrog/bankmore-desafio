using BankMore.Domain.Core.Commands;

namespace BankMore.Application.ContasCorrentes.Commands;

public abstract class ContaCorrenteCommand : Command
{
    public Guid? Id { get; set; }
    public string Nome { get; protected set; }
    public int Numero { get; protected set; }
    public string Cpf { get; protected set; }
    public string Senha { get; protected set; }
    public string SenhaAnterior { get; protected set; }
    public bool Ativo { get; protected set; }
}
