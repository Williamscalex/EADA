using System.ComponentModel.DataAnnotations;

namespace EADA.Core.Domain.Mapped.Expense;

public class ExpenseCategory
{
    [Key]
    public int ExpenseCategoryId { get; set; }
    public string CategoryName { get; set; }
    public bool IsSystemDefault { get; set; }

}