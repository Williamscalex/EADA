using AutoMapper;
using EADA.Core.Contracts;
using EADA.Core.Contracts.ApplicationServices;
using EADA.Core.Domain.Mapped.Expense;
using EADA.Core.Domain.Messages.Args;
using EADA.Core.Domain.Messages.Dto;
using Microsoft.EntityFrameworkCore;
using Rbit.Exceptions;

namespace EADA.Core.ApplicationServices;

public class ExpenseService : IExpenseService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public ExpenseService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Expense>> GetAllExpenses()
    {
            var expenses = await _uow.Expense.Query()
                .Include(x => x.ExpenseType)
                .Include(x => x.ExpenseCategory)
                .ToListAsync();

            return expenses;
    }

    public async Task<Expense> GetExpenseById(int id)
    {
            var expense = await _uow.Expense.Query()
                .Where(x => x.ExpenseId == id)
                .Include(x => x.ExpenseType)
                .Include(x => x.ExpenseCategory)
                .FirstOrDefaultAsync();

            return expense;
    }

    public async Task DeleteExpense(int id)
    {
            if (id < 1) return;
            var expense = await _uow.Expense.Query()
                .Where(x => x.ExpenseId == id)
                .FirstOrDefaultAsync();
            if (expense == null) return;

            _uow.Expense.Remove(expense);

            await _uow.SaveChangesAsync();
    }

    public async Task<Expense> EditExpense(ExpenseArgs args)
    {

            var expense = await GetExpenseById(args.ExpenseId);

            if (expense == null) throw new HandledException($"Expense with id {args.ExpenseId} was not found.");

            _mapper.Map(args, expense);
           _uow.Expense.Update(expense);

            await _uow.SaveChangesAsync();

            return expense;
    }

    public async Task<Expense> CreateExpense(ExpenseArgs args)
    {
            var expense = _mapper.Map<Expense>(args);

            if (expense == null) throw new HandledException("Failed to create new expense.");

            _uow.Expense.Add(expense);
            await _uow.SaveChangesAsync();
            return expense;
    }
}