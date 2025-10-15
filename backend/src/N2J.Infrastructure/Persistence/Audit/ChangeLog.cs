
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

    // Performance and Querying
    public string? EntityState { get; set; } // Serialized entity state (optional)

    // Computed Properties (not stored in DB)
    public IReadOnlyDictionary<string, object> Changes => GetChangesDictionary();

    private Dictionary<string, object> GetChangesDictionary()
    {
        if (string.IsNullOrEmpty(ChangesJson))
            return new Dictionary<string, object>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(ChangesJson)
                ?? new Dictionary<string, object>();
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }
}


