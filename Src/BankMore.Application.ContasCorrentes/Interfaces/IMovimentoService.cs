using BankMore.Application.ContasCorrentes.ViewModels;
namespace BankMore.Application.ContasCorrentes.Interfaces
{
    public interface IMovimentoService : IDisposable
    {
        void Cadastrar(MovimentoViewModel contaCorrenteViewModel);
        Task<bool> Existe(Guid id);
    }
}
