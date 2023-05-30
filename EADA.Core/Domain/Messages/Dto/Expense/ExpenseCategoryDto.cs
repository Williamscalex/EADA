namespace EADA.Core.Domain.Messages.Dto;

public class ExpenseCategoryDto
{
    public int ExpenseCategoryId { get; set; }
    public string CategoryName { get; set; }
    public bool IsSystemDefault { get; set; }
}