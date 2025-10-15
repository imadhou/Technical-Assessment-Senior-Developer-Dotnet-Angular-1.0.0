
namespace N2J.Infrastructure.Persistence.Audit;

public class ChangeLog
{
    public Guid Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public string? ChangesJson { get; set; }
    public Guid? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}


