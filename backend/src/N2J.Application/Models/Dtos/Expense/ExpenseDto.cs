using N2J.Application.Models.Dtos.Expense;
using N2J.Application.Models.Dtos.UserAuth;

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
    public Guid ExpenseCategoryId { get; set; }
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
    public string Status { get; set; } = "Pending";
    /// <summary>
    /// Expense receipt URL.
    /// </summary>
    public string? ReceiptUrl { get; set; }
    /// <summary>
    /// Receipt file name.
    /// </summary>
    public string? ReceiptFileName { get; set; }

    /// <summary>
    /// Size of the receipt file in bytes.
    /// </summary>
    public long? ReceiptFileSize { get; set; }

    /// <summary>
    /// Receipt upload date in UTC.
    /// </summary>
    public DateTime? ReceiptUploadedAt { get; set; }

    /// <summary>
    /// Expense creation date in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Expense last update date in UTC.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public ExpenseCategoryDto ExpenseCategory { get; set; } = null!;
    public UserDto User { get; set; } = null!;

    public List<LinkDto> Links { get; set; } = new List<LinkDto>();
}