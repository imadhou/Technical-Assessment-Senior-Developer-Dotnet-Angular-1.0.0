namespace N2J.Application.Models.Dtos;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<UserDto> Users { get; set; } = new List<UserDto>();
    public ICollection<ExpenseCategoryDto> Categories { get; set; } = new List<ExpenseCategoryDto>();
}
