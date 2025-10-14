namespace N2J.Application.Models.Dtos;

public class ExpenseDto
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// General description of the expense.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Amount of the expense.
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// Expense category identifier.
    /// </summary>
    public Guid CategoryId { get; set; }
    /// <summary>
    /// Expense user identifier.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Expense date in UTC.
    /// </summary>
    public DateTime Date { get; set; }
    /// <summary>
    /// Expense status (Pending, Approved, Rejected).
    /// </summary>
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    /// <summary>
    /// Expense receipt URL.
    /// </summary>
    public string? ReceiptUrl { get; set; }
    /// <summary>
    /// Expense creation date in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Expense last update date in UTC.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public ExpenseCategoryDto Category { get; set; } = null!;
    public UserDto User { get; set; } = null!;
}