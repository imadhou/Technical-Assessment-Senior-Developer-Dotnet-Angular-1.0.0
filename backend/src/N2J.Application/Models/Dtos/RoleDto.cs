namespace N2J.Application.Models.Dtos;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<UserDto> Users { get; set; } = new List<UserDto>();
}
