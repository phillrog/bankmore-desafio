using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Models;

namespace BankMore.Domain.ContasCorrentes.Interfaces;

public interface IIdempotenciaRepository : IRepository<Idempotencia>
{
    bool Exist(Guid id);
}
