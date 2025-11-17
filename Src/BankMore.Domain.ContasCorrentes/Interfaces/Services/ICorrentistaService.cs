using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Models;

namespace BankMore.Domain.ContasCorrentes.Interfaces.Services
{
    public interface ICorrentistaService
    {
        Task<ContaCorrente> BuscarConta(int numeroConta);
        Task<SaldoDetalhadoDto> Saldo(int numeroConta);
    }
}
