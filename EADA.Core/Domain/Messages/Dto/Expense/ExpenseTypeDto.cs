namespace EADA.Core.Domain.Messages.Dto;

public class ExpenseTypeDto
{
    public int ExpenseTypeId { get; set; }
    public string TypeName { get; set; }
    public bool IsSystemDefault { get; set; }
}