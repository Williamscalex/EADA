using System.ComponentModel.DataAnnotations;
using EADA.Core.Constants;

namespace EADA.Core.Domain.Mapped.Expense;

public class Expense
{
    [Key]
    public int ExpenseId { get; set; }
    [StringLength(SqlDataLength.Name)]
    public string ExpenseName { get; set; }
    public decimal? CostPerMonth { get; set; }
    public decimal? CostPerYear { get; set; }
    [StringLength(SqlDataLength.Description)]
    public string Description { get; set; }

    public  int ExpenseTypeId { get; set; }
    public  int ExpenseCategoryId { get; set; }

    #region Navigation Properties

    public virtual ExpenseType ExpenseType { get; set; }
    public virtual ExpenseCategory ExpenseCategory { get; set; }

    #endregion
}