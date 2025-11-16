using BankMore.Application.ContasCorrentes.ViewModels;

namespace BankMore.Application.ContasCorrentes.Interfaces;

public interface IContaCorrenteService : IDisposable
{
    void Cadastrar(ContaCorrenteViewModel contaCorrenteViewModel);
    void Alterar(ContaCorrenteViewModel contaCorrenteViewModel);

}
