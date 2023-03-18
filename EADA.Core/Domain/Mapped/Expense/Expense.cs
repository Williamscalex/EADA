using System.ComponentModel.DataAnnotations;

namespace EADA.Core.Domain.Mapped.Expense;

public class Expense
{
    [Key]
    public int ExpenseId { get; set; }
    [StringLength(50)]
    public string ExpenseName { get; set; }
    public decimal? CostPerMonth { get; set; }
    public decimal? CostPerYear { get; set; }
    [StringLength(150)]
    public string Description { get; set; }

    public  int ExpenseTypeId { get; set; }
    public  int ExpenseCategoryId { get; set; }

    #region Navigation Properties

    public virtual ExpenseType ExpenseType { get; set; }
    public virtual ExpenseCategory ExpenseCategory { get; set; }

    #endregion
}