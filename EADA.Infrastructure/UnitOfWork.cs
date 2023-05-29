using EADA.Core.Contracts;
using EADA.Core.Contracts.Configuration;
using EADA.Core.Domain.Mapped.Expense;
using EADA.Infrastructure.Contexts;
using EADA.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EADA.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private readonly IMainConfiguration _mainConfiguration;
    private readonly int _connectionTimeout;

    private IRepository<Expense> _expenses;
    private IRepository<ExpenseCategory> _expensesCategory;

    public UnitOfWork(
        AppDbContext context,
        IMainConfiguration mainConfiguration)
    {
        _mainConfiguration = mainConfiguration;
        _context = context;
    }

    public IRepository<Expense> Expense => _expenses ??= BuildRepository<Expense>();
    public IRepository<ExpenseCategory> ExpenseCategory =>_expensesCategory ??= BuildRepository<ExpenseCategory>();

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public void SetConnectionTimeout(int seconds)
    {
        throw new NotImplementedException();
    }

    public void ResetConnectionTimeout()
    {
        throw new NotImplementedException();
    }

    public IRepository<TEntity> BuildRepository<TEntity>() where TEntity : class => new Repository<TEntity>(_context);

    public IRepository<TEntity> BuildRepository<TEntity>(IEnumerable<string> excludeEagerEntityPaths) where TEntity : class =>
        new Repository<TEntity>(_context, excludeEagerEntityPaths);
}