namespace N2J.Application.Models.Dtos;

public class ExpenseDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string? ReceiptUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ExpenseCategoryDto Category { get; set; } = null!;
    public virtual UserDto User { get; set; } = null!;
}