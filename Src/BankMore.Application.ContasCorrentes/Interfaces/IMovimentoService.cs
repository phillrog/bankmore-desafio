using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Models;
namespace BankMore.Application.ContasCorrentes.Interfaces
{
    public interface IMovimentoService : IDisposable
    {
        Task<Result<MovimentacaoRelaizadaDto>> Cadastrar(MovimentoViewModel contaCorrenteViewModel);
        Task<bool> Existe(Guid id);
    }
}
