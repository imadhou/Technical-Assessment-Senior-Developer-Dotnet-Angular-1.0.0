namespace N2J.Domain.Entities;

public class Expense
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public Guid ExpenseCategoryId { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public string? ReceiptUrl { get; set; }
    public string? ReceiptFileName { get; set; }
    public long? ReceiptFileSize { get; set; }
    public DateTime? ReceiptUploadedAt { get; set; }

    // Navigation properties
    public virtual ExpenseCategory ExpenseCategory { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}