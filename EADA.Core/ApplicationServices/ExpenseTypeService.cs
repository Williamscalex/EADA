using AutoMapper;
using EADA.Core.Contracts;
using EADA.Core.Contracts.ApplicationServices;
using EADA.Core.Domain.Mapped.Expense;
using EADA.Core.Domain.Messages.Args;
using Microsoft.EntityFrameworkCore;
using Rbit.Exceptions;

namespace EADA.Core.ApplicationServices;

public class ExpenseTypeService : IExpenseTypeService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uow;

    public ExpenseTypeService(IMapper mapper, IUnitOfWork uow)
    {
        _mapper = mapper;
        _uow = uow;
    }


    public async Task<IEnumerable<ExpenseType>> GetAllExpenseTypes()
    {
        return await _uow.ExpenseType.Query().ToListAsync();
    }

    public async Task<ExpenseType> GetExpenseTypeById(int id)
    {
        var type = await _uow.ExpenseType.Query()
            .FirstOrDefaultAsync(x => x.ExpenseTypeId == id && !x.IsSystemDefault);
        if (type == null)
            throw new HandledException($"Failed to retrieve the expense type with id {id}.");

        return type;
    }

    public async Task<ExpenseType> CreateExpenseType(ExpenseTypeArgs args)
    {
        var newType = _mapper.Map<ExpenseType>(args);

        if (newType == null)
            throw new HandledException("Failed to map the new expense type.");

        _uow.ExpenseType.Add(newType);

        await _uow.SaveChangesAsync();

        return newType;
    }

    public async Task<ExpenseType> EditExpenseType(ExpenseTypeArgs args)
    {
        var type = await _uow.ExpenseType.Query()
            .FirstOrDefaultAsync(x => x.ExpenseTypeId == args.ExpenseTypeId && !x.IsSystemDefault);
        if (type == null)
            throw new HandledException($"failed to retrieve the expense type with Id {args.ExpenseTypeId}.");

        var updatedType = _mapper.Map(args, type);

        _uow.ExpenseType.Update(updatedType);

        await _uow.SaveChangesAsync();

        return updatedType;
    }

    public async Task DeleteExpenseType(int id)
    {
        var type = await _uow.ExpenseType.Query()
            .FirstOrDefaultAsync(x => x.ExpenseTypeId == id && !x.IsSystemDefault);

        if (type == null) return;

        _uow.ExpenseType.Remove(type);

        await _uow.SaveChangesAsync();
    }
}