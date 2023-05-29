using EADA.Core.Domain.Mapped.Expense;
using EADA.Core.Domain.Messages.Args;

namespace EADA.Core.Contracts.ApplicationServices;

public interface IExpenseCategoryService
{
    /// <summary>
    /// Retrieves a list of all <see cref="ExpenseCategory"/>
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<ExpenseCategory>> GetAllCategories();
    /// <summary>
    /// Retrieves a <see cref="ExpenseCategory"/> by the provided Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<ExpenseCategory> GetCategoryById(int id);
    /// <summary>
    /// Creates a new <see cref="ExpenseCategory"/>.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public Task<ExpenseCategory> CreateCategory(ExpenseCategoryArgs args);
    /// <summary>
    /// Edits a currently existing <see cref="ExpenseCategory"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public Task<ExpenseCategory> EditCategory(ExpenseCategoryArgs args);
    /// <summary>
    /// Removes a category from the database.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task DeleteCategory(int id);
}