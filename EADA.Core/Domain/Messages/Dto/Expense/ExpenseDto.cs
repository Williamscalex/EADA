using EADA.Core.Domain.Mapped.Expense;

namespace EADA.Core.Domain.Messages.Dto;

public class ExpenseDto
{
    public int ExpenseId { get; set; }
    public int ExpenseCategoryId { get; set; }
    public string ExpenseName { get; set; }
    public decimal? CostPerMonth { get; set; }
    public decimal? CostPerYear { get; set; }
    public string Description { get; set; }
    public ExpenseType ExpenseType { get; set; }
    public ExpenseCategory ExpenseCategory { get; set; }
}