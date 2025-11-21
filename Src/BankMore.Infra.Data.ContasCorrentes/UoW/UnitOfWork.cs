using BankMore.Domain.Common.Interfaces;
using BankMore.Infra.Data.ContasCorrentes.Context;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;

namespace BankMore.Infra.Data.ContasCorrentes.UoW;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private TransactionScope? _currentScope;
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction is null)
        {
            _currentScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }
    }
    public bool Commit()
    {
        try
        {
            _context.SaveChanges();
            _currentTransaction?.Commit();

            _currentTransaction?.Dispose();
            _currentTransaction = null;
            return true;
        }
        catch (Exception)
        {
            RollbackTransaction();
            return false;
        }
    }

    public void RollbackTransaction()
    {
        if (_currentTransaction != null)
        {
            _currentTransaction.Rollback();
            _currentTransaction.Dispose();
        }
        _currentTransaction = null;
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
