using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Transferencias.Models;

namespace BankMore.Domain.Transferencias.Interfaces;

public interface IIdempotenciaRepository : IRepository<Idempotencia>
{
    bool Exist(Guid id);
}
