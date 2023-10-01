using AutoMapper;
using EADA.Core.Contracts.ApplicationServices;
using EADA.Core.Domain.Mapped.Expense;
using EADA.Core.Domain.Messages.Args;
using EADA.Core.Domain.Messages.Dto;
using EADA.Core.FluentValidatorExtensions.Enums;
using EADA.Core.FluentValidatorExtensions.Helpers.Extensions;
using EADA.Web.Helpers.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Rbit.Exceptions;

namespace EADA.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpenseController: ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly IMapper _mapper;
    private readonly IValidator<ExpenseArgs> _validator;

    public ExpenseController(IExpenseService expenseService, IMapper mapper, IValidator<ExpenseArgs> validator)
    {
        _expenseService = expenseService;
        _mapper = mapper;
        _validator = validator;
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> GetExpenses()
    {
        try
        {
            var expenses = await _expenseService.GetAllExpenses();

            var dtos = expenses.Select(x => _mapper.Map<ExpenseDto>(x));

            return Ok(dtos);
        }
        catch (Exception e)
        {
            return this.handledError(e, "Failed to retrieve expenses.");
        }

    }

    [Route("{id:min(1)}")]
    [HttpGet]
    public async Task<IActionResult> GetExpenseById([FromRoute] int id)
    {
        try
        {
            if (id <= 0) throw new HandledException($"An expense with Id: {id} could not be.");
            var expense = await _expenseService.GetExpenseById(id);

            var dto = _mapper.Map<ExpenseDto>(expense);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return this.handledError(e, $"Failed to retrieve expense with id {id}.");
        }
    }

    [Route("edit")]
    [HttpPost]
    public async Task<IActionResult> EditExpense([FromBody] ExpenseArgs expenseArgs)
    {
        try
        {
            await _validator.ValidateCommonAsync(expenseArgs, CommonRuleSet.Edit);

            var result = await _expenseService.EditExpense(expenseArgs);

            var dto = _mapper.Map<ExpenseDto>(result);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return this.handledError(e, $"Failed to edit the expense.");
        }
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] ExpenseArgs expenseArgs)
    {
        try
        {
            await _validator.ValidateCommonAsync(expenseArgs, CommonRuleSet.Create);

            var result = await _expenseService.CreateExpense(expenseArgs);

            var dto = _mapper.Map<ExpenseDto>(result);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return this.handledError(e, "Failed to create new expense.");
        }
    }

    [Route("{id:min(1)}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteExpense([FromRoute] int id)
    {
        try
        {
            await _expenseService.DeleteExpense(id);
            return Ok();
        }
        catch (Exception e)
        {
            return this.handledError(e,
                $"An unexpected error occurred while attempting to delete the expense with id {id}");
        }
    }
}