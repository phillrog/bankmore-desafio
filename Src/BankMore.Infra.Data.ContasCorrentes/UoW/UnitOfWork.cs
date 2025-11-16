using BankMore.Domain.Common.Interfaces;
using BankMore.Infra.Data.ContasCorrentes.Context;

namespace BankMore.Infra.Data.ContasCorrentes.UoW;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool Commit()
    {
        return _context.SaveChanges() > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
