using System.ComponentModel.DataAnnotations;

namespace EADA.Core.Domain.Mapped.Expense;

public class Expense
{
    [Key]
    public int ExpenseId { get; set; }
    public string ExpenseName { get; set; }
    public decimal CostPerMonth { get; set; }

    public  int ExpenseTypeId { get; set; }
    public  int ExpenseCategoryId { get; set; }

    #region Navigation Properties

    public virtual ExpenseType ExpenseType { get; set; }
    public virtual ExpenseCategory ExpenseCategory { get; set; }

    #endregion
}