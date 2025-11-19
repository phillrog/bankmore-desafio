using BankMore.Application.Transferencias.ViewModels;
using BankMore.Domain.Core.Models;

namespace BankMore.Application.Transferencias.Interfaces;

public interface IIdempotenciaService : IDisposable
{
    Task<Result<bool>> Cadastrar(IdempotenciaViewModel contaCorrenteViewModel);
    Task<bool> Existe(Guid id);
}
