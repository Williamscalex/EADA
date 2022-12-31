using EADA.Core.Domain.Mapped.Expense;

namespace EADA.Core.Contracts;

public interface IUnitOfWork
{
    #region Repositories

    IRepository<Expense> Expense { get; }

    #endregion

    int SaveChanges();
    Task<int> SaveChangesAsync();
    /// <summary>
    /// Sets the connection timeout to use when querying the database from this instance.
    /// </summary>
    /// <param name="seconds"></param>
    void SetConnectionTimeout(int seconds);
    /// <summary>
    /// Resets the connection timeout to the default value for this instance.
    /// </summary>
    void ResetConnectionTimeout();
}