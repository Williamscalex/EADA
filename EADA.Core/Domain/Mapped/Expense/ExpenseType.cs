using System.ComponentModel.DataAnnotations;

namespace EADA.Core.Domain.Mapped.Expense;

public class ExpenseType
{
    [Key]
    public int ExpenseTypeId { get; set; }
    public string TypeName { get; set; }
}