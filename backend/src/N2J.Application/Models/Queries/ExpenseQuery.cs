using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace N2J.Application.Models.Queries;

/// <summary>
/// Query parameters for filtering and paginating expenses
/// </summary>
public class ExpenseQuery : PaginatedQuery
{
    /// <summary>
    /// Filter by description (case-insensitive contains search)
    /// </summary>
    /// <example>lunch</example>
    public string? Description { get; set; }

    /// <summary>
    /// Filter by start date (inclusive)
    /// </summary>
    /// <example>2024-01-01</example>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Filter by end date (inclusive)
    /// </summary>
    /// <example>2024-01-31</example>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Filter by minimum amount (inclusive)
    /// </summary>
    /// <example>10.00</example>
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// Filter by maximum amount (inclusive)
    /// </summary>
    /// <example>1000.00</example>
    public decimal? MaxAmount { get; set; }

    /// <summary>
    /// Filter by expense category ID
    /// </summary>
    /// <example>12345678-1234-1234-1234-123456789abc</example>
    public Guid? ExpenseCategoryId { get; set; }

    /// <summary>
    /// Filter by expense user ID
    /// </summary>
    /// <example>12345678-1234-1234-1234-123456789abc</example>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Filter by expense status (e.g., "Pending", "Approved", "Rejected")
    /// </summary>
    /// <example>Approved</example>
    public string? Status { get; set; }
}