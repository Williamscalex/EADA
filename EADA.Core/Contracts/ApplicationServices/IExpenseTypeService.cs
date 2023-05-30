using EADA.Core.Domain.Mapped.Expense;
using EADA.Core.Domain.Messages.Args;

namespace EADA.Core.Contracts.ApplicationServices;

public interface IExpenseTypeService
{
    /// <summary>
    /// Retrieves all <see cref="ExpenseType"/>
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<ExpenseType>> GetAllExpenseTypes();
    /// <summary>
    /// Retrieves a single <see cref="ExpenseType"/> by its specified id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<ExpenseType> GetExpenseTypeById(int id);
    /// <summary>
    /// Creates a new <see cref="ExpenseType"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public Task<ExpenseType> CreateExpenseType(ExpenseTypeArgs args);
    /// <summary>
    /// Edits an existing <see cref="ExpenseType"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public Task<ExpenseType> EditExpenseType(ExpenseTypeArgs args);
    /// <summary>
    /// Removes the specified <see cref="ExpenseType"/> using its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task DeleteExpenseType(int id);
}