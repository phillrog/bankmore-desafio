using BankMore.Application.ContasCorrentes.ViewModels;

namespace BankMore.Application.ContasCorrentes.Interfaces;

public interface IIdempotenciaService : IDisposable
{
    void Cadastrar(IdempotenciaViewModel contaCorrenteViewModel);
    Task<bool> Existe(Guid id);
}
