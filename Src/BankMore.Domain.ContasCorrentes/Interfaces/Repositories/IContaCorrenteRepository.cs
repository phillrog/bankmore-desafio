using BankMore.Domain.Common.Dtos;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Models;


namespace BankMore.Domain.ContasCorrentes.Interfaces;

public interface IContaCorrenteRepository : IRepository<ContaCorrente>
{
    ContaCorrente GetByCpf(string cpf);
    ContaCorrente GetByNumero(int numero);
    Task<SaldoDto> BuscarSaldoPorNumeroAsync(int numeroConta);
    Task<SaldoDto> BuscarSaldoPorIdAsync(Guid id);
}