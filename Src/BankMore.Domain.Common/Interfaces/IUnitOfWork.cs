using System;

namespace BankMore.Domain.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    bool Commit();
}
