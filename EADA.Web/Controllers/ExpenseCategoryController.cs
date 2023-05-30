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

[Route("api/[controller]")]
[ApiController]
public class ExpenseCategoryController : ControllerBase
{
    private readonly IExpenseCategoryService _expenseCategoryService;
    private readonly IMapper _mapper;
    private readonly IValidator<ExpenseCategoryArgs> _validator;
    public ExpenseCategoryController(IExpenseCategoryService expenseCategoryService, IMapper mapper, IValidator<ExpenseCategoryArgs> validator)
    {
        _expenseCategoryService = expenseCategoryService;
        _mapper = mapper;
        _validator = validator;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            var expenses = await _expenseCategoryService.GetAllCategories();

            var dtos = expenses.Select(x => _mapper.Map<ExpenseCategoryDto>(x));

            return Ok(dtos);
        }
        catch (Exception e)
        {
            return this.handledError(e, "Failed to retrieve expense categories.");
        }
    }

    [HttpGet]
    [Route("{id:min(1)}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        try
        {
            var expense = await _expenseCategoryService.GetCategoryById(id);

            var dto = _mapper.Map<ExpenseCategoryDto>(expense);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return this.handledError(e, $"Failed to retrieve expense category with id {id}.");
        }
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateCategory(ExpenseCategoryArgs args)
    {
        try
        {
            await _validator.ValidateCommonAsync(args, CommonRuleSet.Create);

            var expense = await _expenseCategoryService.CreateCategory(args);

            var dto = _mapper.Map<ExpenseCategoryDto>(expense);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return this.handledError(e, "Failed to create expense.");
        }
    }

    [HttpPost]
    [Route("edit")]
    public async Task<IActionResult> EditCategory(ExpenseCategoryArgs args)
    {
        try
        {
            await _validator.ValidateCommonAsync(args, CommonRuleSet.Edit);

            var expense = await _expenseCategoryService.EditCategory(args);

            var dto = _mapper.Map<ExpenseCategoryDto>(expense);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return this.handledError(e, "Failed to edit expense category.");
        }
    }

    [HttpDelete]
    [Route("{id:min(1)}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            await _expenseCategoryService.DeleteCategory(id);

            return Ok();
        }
        catch (Exception e)
        {
            return this.handledError(e, "Failed to delete expense category.");
        }
    }
}