using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.ContasCorrentes.Models;
using BankMore.Infra.Data.Common.Repository;
using BankMore.Infra.Data.ContasCorrentes.Context;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Infra.Data.ContasCorrentes.Repository
{
    public class MovimentoRepository : Repository<Movimento, ApplicationDbContext>, IMovimentoRepository
    {
        public MovimentoRepository(ApplicationDbContext context)
       : base(context)
        {
        }

        public bool Exist(Guid id)
        {
            return _dbSet.AsNoTracking().Any(x => x.Id == id);
        }
    }
}
