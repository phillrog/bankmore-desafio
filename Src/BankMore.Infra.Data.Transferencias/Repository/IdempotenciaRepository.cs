using BankMore.Domain.Transferencias.Interfaces;
using BankMore.Domain.Transferencias.Models;
using BankMore.Infra.Data.Common.Repository;
using BankMore.Infra.Data.Transferencias.Context;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Infra.Data.Transferencias.Repository
{
    public class IdempotenciaRepository : Repository<Idempotencia, ApplicationDbContext>, IIdempotenciaRepository
    {
        public IdempotenciaRepository(ApplicationDbContext context)
        : base(context)
        {
        }

        public bool Exist(Guid id)
        {
            return _dbSet.AsNoTracking().Any(x => x.Id == id);
        }
    }
}
