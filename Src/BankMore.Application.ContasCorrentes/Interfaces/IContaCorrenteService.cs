using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Models;

namespace BankMore.Application.ContasCorrentes.Interfaces;

public interface IContaCorrenteService : IDisposable
{
    Task<Result<NumeroContaCorrenteDto>> Cadastrar(NovaCorrenteViewModel contaCorrenteViewModel);
    void Alterar(ContaCorrenteViewModel contaCorrenteViewModel);
    Task<Result<InformacoesViewModel>> BuscarInformcoes(int numero);
    Task<InformacoesViewModel> BuscarPorNumero(int numero);
    Task<SaldoDto> Saldo(int numero);
}
