namespace N2J.Application.Models.Dtos;

public class ExpenseCategoryDto
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Expense category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Expense detailed description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// The company identifier to which this category belongs.
    /// </summary>
    public Guid CompanyId { get; set; }
    /// <summary>
    /// Creation date of the expense category in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
