using AutoMapper;
using EADA.Core.Contracts;
using EADA.Core.Contracts.ApplicationServices;
using EADA.Core.Domain.Mapped.Expense;
using EADA.Core.Domain.Messages.Args;
using Microsoft.EntityFrameworkCore;
using Rbit.Exceptions;

namespace EADA.Core.ApplicationServices;

public class ExpenseCategoryService : IExpenseCategoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ExpenseCategoryService(IMapper mapper, IUnitOfWork uow)
    {
        _mapper = mapper;
        _uow = uow;
    }

    public async Task<IEnumerable<ExpenseCategory>> GetAllCategories()
    {
        return await _uow.ExpenseCategory.Query().ToListAsync();
    }

    public async Task<ExpenseCategory> GetCategoryById(int id)
    {
        var expense = await _uow.ExpenseCategory.Query()
            .FirstOrDefaultAsync(x => x.ExpenseCategoryId == id && !x.IsSystemDefault);
        if (expense == null) 
            throw new HandledException($"Failed to retrieve the expense category with {id}.");

        return expense;
    }

    public async Task<ExpenseCategory> CreateCategory(ExpenseCategoryArgs args)
    {
        var expense = _mapper.Map<ExpenseCategory>(args);

        if (expense == null)
            throw new HandledException("Failed to create new expense.");

        _uow.ExpenseCategory.Add(expense);

        await _uow.SaveChangesAsync();

        return expense;
    }

    public async Task<ExpenseCategory> EditCategory(ExpenseCategoryArgs args)
    {
        var expense = await _uow.ExpenseCategory.Query()
            .FirstOrDefaultAsync(x => x.ExpenseCategoryId == args.ExpenseCategoryId && !x.IsSystemDefault);

        if (expense == null)
            throw new HandledException($"Failed to retrieve expense category with id {args.ExpenseCategoryId}");

        var editExpense = _mapper.Map(args, expense);

        _uow.ExpenseCategory.Update(editExpense);

        await _uow.SaveChangesAsync();

        return editExpense;
    }

    
    public async Task DeleteCategory(int id)
    {
        var expense = await _uow.ExpenseCategory.Query()
            .FirstOrDefaultAsync(x => x.ExpenseCategoryId == id && !x.IsSystemDefault);

        if (expense == null) return;

        _uow.ExpenseCategory.Remove(expense);

        await _uow.SaveChangesAsync();
    }
}