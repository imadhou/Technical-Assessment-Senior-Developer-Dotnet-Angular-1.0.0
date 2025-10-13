namespace N2J.Application.Models.Dtos;

public class ExpenseCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual CompanyDto Company { get; set; } = null!;
    public virtual ICollection<ExpenseDto> Expenses { get; set; } = new List<ExpenseDto>();
}
