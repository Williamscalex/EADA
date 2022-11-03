using EADA.Core.Domain.Mapped.Expense;
using EADA.Core.Domain.Messages.Args;

namespace EADA.Core.Contracts.ApplicationServices;

public interface IExpenseService
{
    /// <summary>
    /// Retrieves all expenses
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<Expense>> GetAllExpenses();
    /// <summary>
    /// Gets a single expense by Id
    /// </summary>
    /// <param name="id">The id of the expense you wish to retrieve.</param>
    /// <returns></returns>
    public Task<Expense> GetExpenseById(int id);
    /// <summary>
    /// Deletes an expsense using it's id
    /// </summary>
    /// <param name="id">Id of the expense you wish to delete.</param>
    /// <returns></returns>
    public Task DeleteExpense(int id);
    /// <summary>
    /// Edits an expense.
    /// </summary>
    /// <param name="args">the object that has the data for editing</param>
    /// <returns></returns>
    public Task<Expense> EditExpense(ExpenseArgs args);
    /// <summary>
    /// Creates a new expense.
    /// </summary>
    /// <param name="args">The object that will be used to create a new expense.</param>
    /// <returns></returns>
    public Task<Expense> CreateExpense(ExpenseArgs args);

}