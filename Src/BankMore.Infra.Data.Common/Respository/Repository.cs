using System.Linq.Expressions;
using BankMore.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Infra.Data.Common.Repository;

public abstract class Repository<TEntity, TContext> : IRepository<TEntity> where TContext : DbContext
    where TEntity : class
{
    protected readonly TContext _db;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(TContext context)
    {
        _db = context;
        _dbSet = _db.Set<TEntity>();
    }

    public virtual void Add(TEntity obj)
    {
        _dbSet.Add(obj);
    }

    public virtual TEntity GetById(Guid id)
    {
        return _dbSet.Find(id);
    }

    public virtual TEntity GetByExpression(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.Find(predicate);
    }

    public virtual IQueryable<TEntity> GetAll()
    {
        return _dbSet;
    }

    public virtual IQueryable<TEntity> GetAll(ISpecification<TEntity> spec)
    {
        return ApplySpecification(spec);
    }

    public virtual IQueryable<TEntity> GetAllSoftDeleted()
    {
        return _dbSet.IgnoreQueryFilters()
            .Where(e => EF.Property<bool>(e, "IsDeleted") == true);
    }

    public virtual void Update(TEntity obj)
    {
        _dbSet.Update(obj);
    }

    public virtual void Remove(Guid id)
    {
        _dbSet.Remove(_dbSet.Find(id));
    }

    public int SaveChanges()
    {
        return _db.SaveChanges();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        return SpecificationEvaluator<TEntity>.GetQuery(_dbSet.AsQueryable(), spec);
    }
}
