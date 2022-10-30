using System.Linq.Expressions;

namespace EADA.Core.Contracts;

public interface IRepository<TEntity> where TEntity : class
{
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
    /// <summary>
    /// Adds the given <typeparamref name="TEntity"/> object as a record in the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    int Count();
    Task<int> CountAsync();
    IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> FindAllAsync();
    TEntity FindSingleOrDefault(Expression<Func<TEntity, bool>> predicate);
    TEntity Get(params object[] id);
    IEnumerable<TEntity> GetAll();
    Task<IEnumerable<TEntity>> GetAllAsync();
    /// <summary>
    /// Exposes the <see cref="IQueryable"/> object for use throughout the application.
    /// </summary>
    /// <returns></returns>
    IQueryable<TEntity> Query();
    /// <summary>
    /// Exposes the <see cref="IQueryable"/> object (modified to exclude disabled entries) for use throughout the application
    /// </summary>
    /// <returns></returns>
    IQueryable<TEntity> QueryIsActive();

}