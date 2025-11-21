using BankMore.Domain.Common.Dtos;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.ContasCorrentes.Interfaces.Services;
using BankMore.Domain.ContasCorrentes.Models;


namespace BankMore.Domain.ContasCorrentes.Services
{
    public class CorrentistaService : ICorrentistaService
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public CorrentistaService(IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<ContaCorrente> BuscarConta(int numeroConta)
        {
            return await _contaCorrenteRepository.GetByExpressionAsync(d => d.Numero == numeroConta);
        }

        public async Task<ContaCorrente> BuscarContaPorId(Guid id)
        {
            return await _contaCorrenteRepository.GetByExpressionAsync(d => d.Id == id);
        }

        public async Task<SaldoDto> Saldo(int numeroConta)
        {
            return await _contaCorrenteRepository.BuscarSaldoPorNumeroAsync(numeroConta);
        }

        public async Task<SaldoDto> SaldoPorId(Guid id)
        {
            return await _contaCorrenteRepository.BuscarSaldoPorIdAsync(id);
        }
    }
}
