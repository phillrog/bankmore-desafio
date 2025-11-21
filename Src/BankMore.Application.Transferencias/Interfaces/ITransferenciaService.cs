using BankMore.Application.Transferencias.ViewModels;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Transferencias.Dtos;

namespace BankMore.Application.Transferencias.Interfaces;

public interface ITransferenciaService : IDisposable
{
    Task<Result<TransferenciaDto>> Cadastrar(RealizarTransferenciaViewModel realizarTransferenciaViewModel);

}
