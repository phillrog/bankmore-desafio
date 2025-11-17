using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Core.Models;

namespace BankMore.Application.ContasCorrentes.Interfaces;

public interface IContaCorrenteService : IDisposable
{
    void Cadastrar(ContaCorrenteViewModel contaCorrenteViewModel);
    void Alterar(ContaCorrenteViewModel contaCorrenteViewModel);
    Task<Result<InformacoesViewModel>> BuscarInformcoes(string cpf);
}
