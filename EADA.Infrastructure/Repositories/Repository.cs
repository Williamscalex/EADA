using System.Linq.Expressions;
using EADA.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using Rbit.EntityFramework.Extensions;

namespace EADA.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _entities;
    private readonly IQueryable<TEntity> _queryBase;

    public Repository(DbContext context, IEnumerable<string> excludingEagerLoadingPaths = null)
    {
        _context = context;
        _entities = context.Set<TEntity>();
        _queryBase = BuildQueryBase(excludingEagerLoadingPaths);
    }

    /// <summary>
    /// Applies the eager includes (omitting any specified exclusion paths) to the entity set and returns the resulting queryable object.
    /// </summary>
    /// <param name="excludeEagerEntityPath"> Paths to prevent eager loading.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private IQueryable<TEntity> BuildQueryBase(IEnumerable<string> excludeEagerEntityPath)
    {
        if (_context == null)
            throw new InvalidOperationException(
                $"Cannot build query base in repository before the local {nameof(_context)} property is set. Are you calling this function before you set the readonly context property> Or is the injected context null?");
        var eagerEntityPaths = _context.GetEagerIncludePaths(typeof(TEntity)).ToList();
        if (eagerEntityPaths != null)
        {
            foreach (var pathToExclude in excludeEagerEntityPath)
            {
                eagerEntityPaths.RemoveAll(x => pathToExclude == x || x.StartsWith(pathToExclude + "."));
            }
        }

        return eagerEntityPaths.Aggregate(_entities,
            (Func<IQueryable<TEntity>, string, IQueryable<TEntity>>)((query, path) => query.Include(path)));
    }

    protected virtual IQueryable<TEntity> QueryBase => _queryBase;

    protected virtual IQueryable<TEntity> QueryIsActiveBase => _queryBase;

    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate) =>
        await QueryIsActiveBase.AnyAsync(predicate);


    public void Add(TEntity entity)
    {
        _entities.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        _entities.AddRange(entities);
    }

    public void Update(TEntity entity)
    {
        _entities.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        _entities.UpdateRange(entities);
    }

    public void Remove(TEntity entity)
    {
        _entities.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _entities.RemoveRange(entities);
    }

    public int Count()
    {
        return _entities.Count();
    }

    public virtual async Task<int> CountAsync() => await _entities.CountAsync();

    public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) =>
        QueryBase.Where(predicate).ToList();

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate) =>
        await QueryBase.Where(predicate).ToListAsync();


    public virtual async Task<IEnumerable<TEntity>> FindAllAsync() => await QueryBase.ToListAsync();

    public virtual TEntity FindSingleOrDefault(Expression<Func<TEntity, bool>> predicate) =>
        QueryBase.SingleOrDefault(predicate);

    public virtual TEntity Get(params object[] id) => _entities.Find(id);

    public virtual IEnumerable<TEntity> GetAll() => _entities.ToList();

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync() => await _entities.ToListAsync();

    public virtual IQueryable<TEntity> Query() => QueryBase;

    public virtual IQueryable<TEntity> QueryIsActive() => QueryIsActiveBase;
}