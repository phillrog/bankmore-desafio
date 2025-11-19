using BankMore.Domain.Transferencias.Dtos;
using BankMore.Domain.Core.Models;
using BankMore.Application.Transferencias.ViewModels;

namespace BankMore.Application.Transferencias.Interfaces;

public interface ITransferenciaService : IDisposable
{
    Task<Result<TransferenciaDto>> Cadastrar(RealizarTransferenciaViewModel realizarTransferenciaViewModel);
   
}
