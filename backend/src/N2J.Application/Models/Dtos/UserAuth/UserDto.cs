using N2J.Application.Models.Dtos.Company;

namespace N2J.Application.Models.Dtos.UserAuth;

public class UserDto
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// Company identifier to which the user belongs.
    /// </summary>
    public Guid CompanyId { get; set; }
    /// <summary>
    /// User's role identifier.
    /// </summary>
    public Guid RoleId { get; set; }
    /// <summary>
    /// Creation date of the user in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Last update date of the user in UTC.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public CompanyDto Company { get; set; } = null!;
    public RoleDto Role { get; set; } = null!;
}
