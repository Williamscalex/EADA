using AutoMapper;
using EADA.Core.Contracts.ApplicationServices;
using EADA.Core.Domain.Messages.Args;
using EADA.Core.Domain.Messages.Dto;
using EADA.Core.FluentValidatorExtensions.Enums;
using EADA.Core.FluentValidatorExtensions.Helpers.Extensions;
using EADA.Web.Helpers.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EADA.Web.Controllers;

public class ExpenseTypeController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IExpenseTypeService _expenseTypeService;
    private readonly IValidator<ExpenseTypeArgs> _validator;

    public ExpenseTypeController(IMapper mapper, IExpenseTypeService expenseTypeService, IValidator<ExpenseTypeArgs> validator)
    {
        _mapper = mapper;
        _expenseTypeService = expenseTypeService;
        _validator = validator;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllExpenseTypes()
    {
        try
        {
            var types = await _expenseTypeService.GetAllExpenseTypes();

            var dtos = types.Select(x => _mapper.Map<ExpenseTypeDto>(x));

            return Ok(dtos);
        }
        catch (Exception e)
        {
            return this.handledError(e, "Failed to retrieve expense types.");
        }
    }

    [HttpGet]
    [Route("{id:min(1)}")]
    public async Task<IActionResult> GetExpenseTypeById(int id)
    {
        try
        {
            var type = await _expenseTypeService.GetExpenseTypeById(id);

            var dto = _mapper.Map<ExpenseTypeDto>(type);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return this.handledError(e, $"Failed to retrieve expense type with id {id}.");
        }
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateExpenseType(ExpenseTypeArgs args)
    {
        try
        {
            await _validator.ValidateCommonAsync(args, CommonRuleSet.Create);

            var type = await _expenseTypeService.CreateExpenseType(args);

            var dto = _mapper.Map<ExpenseTypeDto>(type);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return this.handledError(e, "Failed to create expense type.");
        }
    }

    [HttpPost]
    [Route("edit")]
    public async Task<IActionResult> EditExpenseType(ExpenseTypeArgs args)
    {
        try
        {
            await _validator.ValidateCommonAsync(args, CommonRuleSet.Edit);

            var type = await _expenseTypeService.EditExpenseType(args);

            var dto = _mapper.Map<ExpenseTypeDto>(type);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return this.handledError(e, $"Failed to edit expense with id {args.ExpenseTypeId}.");
        }
    }

    [HttpDelete]
    [Route("")]
    public async Task<IActionResult> DeleteExpenseType(int id)
    {
        try
        {
            await _expenseTypeService.DeleteExpenseType(id);

            return Ok();
        }
        catch (Exception e)
        {
            return this.handledError(e, "Failed to delete expense type.");
        }
    }
}