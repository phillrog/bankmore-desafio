using BankMore.Domain.Transferencias.Interfaces;
using BankMore.Domain.Transferencias.Models;
using BankMore.Infra.Data.Common.Repository;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Infra.Data.Transferencias.Repository;

public class TransferenciaRepository : Repository<Transferencia, ApplicationDbContext>, ITransferenciaRepository
{
    public TransferenciaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task AtualizarStatusAsync(Transferencia transferencia)
    {
        _dbSet.Update(transferencia);

        return Task.CompletedTask;
    }

    public bool Exist(Guid id)
    {
        return _dbSet.AsNoTracking().Any(x => x.Id == id);
    }
}
