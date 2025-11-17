using System;
using System.Linq.Expressions;

namespace BankMore.Domain.Common.Interfaces;

public interface IRepository<TEntity> : IDisposable
    where TEntity : class
{
    void Add(TEntity obj);

    TEntity GetById(Guid id);

    Task<TEntity?> GetByExpressionAsync(Expression<Func<TEntity, bool>> predicate);

    IQueryable<TEntity> GetAll();

    IQueryable<TEntity> GetAll(ISpecification<TEntity> spec);

    IQueryable<TEntity> GetAllSoftDeleted();

    void Update(TEntity obj);

    void Remove(Guid id);

    int SaveChanges();
}
