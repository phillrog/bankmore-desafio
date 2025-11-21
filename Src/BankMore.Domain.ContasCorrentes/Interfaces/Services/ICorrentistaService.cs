using BankMore.Domain.Common.Dtos;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Models;

namespace BankMore.Domain.ContasCorrentes.Interfaces.Services
{
    public interface ICorrentistaService
    {
        Task<ContaCorrente> BuscarConta(int numeroConta);
        Task<ContaCorrente> BuscarContaPorId(Guid id);
        Task<SaldoDto> Saldo(int numeroConta);
        Task<SaldoDto> SaldoPorId(Guid id);
    }
}
