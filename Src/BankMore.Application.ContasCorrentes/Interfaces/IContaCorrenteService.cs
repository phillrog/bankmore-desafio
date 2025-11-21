
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Common;
using BankMore.Domain.Common.Dtos;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Models;

namespace BankMore.Application.ContasCorrentes.Interfaces;

public interface IContaCorrenteService : IDisposable
{
    Task<Result<NumeroContaCorrenteDto>> Cadastrar(NovaCorrenteViewModel contaCorrenteViewModel);
    void Alterar(ContaCorrenteViewModel contaCorrenteViewModel);
    Task<Result<InformacoesContaCorrenteDto>> BuscarInformcoes(int numero);
    Task<InformacoesContaCorrenteDto> BuscarPorNumero(int numero);
    Task<SaldoDto> Saldo(int numero);
}
