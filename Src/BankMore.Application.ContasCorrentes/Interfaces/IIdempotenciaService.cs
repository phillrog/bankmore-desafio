using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Core.Models;

namespace BankMore.Application.ContasCorrentes.Interfaces;

public interface IIdempotenciaService : IDisposable
{
    Task<Result<bool>> Cadastrar(IdempotenciaViewModel contaCorrenteViewModel);
    Task<bool> Existe(Guid id);
}
