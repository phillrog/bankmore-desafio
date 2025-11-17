using BankMore.Domain.Core.Models;

namespace BankMore.Domain.ContasCorrentes.Models
{
    public class ContaCorrente : EntityAudit
    {
        public string Nome { get; private set; }
        public int Numero { get; private set; }
        public string Cpf { get; private set; }
        public string Senha { get; private set; }
        public string Salt { get; private set; }
        public bool Ativo { get; private set; }
        public ICollection<Idempotencia>? Idempotencias { get; set; }
        public ICollection<Movimento>? Movimentacoes { get; set; }

        public ContaCorrente() { }

        public ContaCorrente(Guid id, string nome, int numero, string cpf, string senha)
        {
            Id = id;
            Nome = nome;
            Numero = numero;
            Cpf = cpf;
            Senha = senha;
            Ativo = true;
        }

        public void AtualizarNome(string nome)
        {
            Nome = nome;
        }

        public void Status(bool status)
        {
            Ativo = status;
        }

        public void AtualizarSenha(string senha)
        {
            Senha = senha;
        }

        public bool Inativa() => !Ativo;
    }
}
