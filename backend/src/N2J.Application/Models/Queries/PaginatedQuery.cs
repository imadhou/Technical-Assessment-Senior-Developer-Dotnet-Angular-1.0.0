using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace N2J.Application.Models.Queries;

public class PaginatedQuery
{
    /// <summary>
    /// Page number (starting from 1). Default is 1.
    /// </summary>
    /// <example>1</example>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    [DefaultValue(1)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Default is 10, maximum is 100.
    /// </summary>
    /// <example>10</example>
    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Property to sort by. Available options: Date, Amount, Description, Category, CreatedAt
    /// </summary>
    /// <example>Date</example>
    [StringLength(50, ErrorMessage = "SortBy cannot exceed 50 characters")]
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort in descending order. Default is false (ascending).
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool IsDescending { get; set; } = false;

    [JsonIgnore]
    public int Take => PageSize;

    [JsonIgnore]
    public int Skip => (Page - 1) * PageSize;
}