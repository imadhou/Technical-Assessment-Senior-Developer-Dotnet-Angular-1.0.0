namespace N2J.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
