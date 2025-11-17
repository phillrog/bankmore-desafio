using System;

namespace BankMore.Domain.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync();

    bool Commit();

    void RollbackTransaction();
}
