using BankMore.Domain.Common.Dtos;
using BankMore.Domain.Core.Models;

namespace BankMore.Domain.Common.Interfaces
{
    public interface ISaldoService
    {
        Task<Result<SaldoDto>> ConsultarSaldo(int numero);
    }
}
