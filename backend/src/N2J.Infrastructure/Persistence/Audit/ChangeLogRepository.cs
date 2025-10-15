using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using N2J.Infrastructure.Persistence.Extensions;
using System.Text.Json;

namespace N2J.Infrastructure.Persistence.Audit;

public class ChangeLogRepository: IChangeLogRepository
{
    private readonly AppDbContext _context;
    public ChangeLogRepository(AppDbContext context)
    {
        _context = context;
    }

    private readonly Guid SystemUserId = Guid.Parse("00000000-0000-0000-0000-000000000000");

    public async Task LogChangeWithTrackingAsync<T>(T entity, string changeType, Guid? changedBy, EntityEntry<T> entityEntry) where T : class
    {
        // Use the extension method to automatically get property changes
        var changes = entityEntry.GetPropertyChanges();

        await LogChangeAsync(
            entity: entity,
            changeType: changeType,
            changedBy: changedBy,
            changes: changes
        );
    }

    public async Task LogChangeAsync<T>(T entity, string changeType, Guid? changedBy, Dictionary<string, object>? changes = null) where T : class
    {
        var entityId = GetEntityId(entity);
        var entityName = typeof(T).Name;


        var changeLog = new ChangeLog
        {
            Id = Guid.NewGuid(),
            EntityName = entityName,
            EntityId = entityId,
            ChangeType = changeType.ToUpper(),
            ChangedBy = changedBy ?? SystemUserId,
            ChangedAt = DateTime.UtcNow,
            ChangesJson = changes != null ? JsonSerializer.Serialize(changes, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }) : null,
        };

        await _context.ChangeLogs.AddAsync(changeLog);
    }

    public async Task<List<ChangeLog>> GetEntityHistoryAsync(string entityName, Guid entityId, int? limit = null)
    {
        var query = _context.ChangeLogs
            .Where(c => c.EntityName == entityName && c.EntityId == entityId)
            .OrderByDescending(c => c.ChangedAt);

        if (limit.HasValue)
        {
            query = (IOrderedQueryable<ChangeLog>)query.Take(limit.Value);
        }

        return await query.ToListAsync();
    }

    public Guid GetEntityId<T>(T entity) where T : class
    {
        var entityEntry = _context.Entry(entity);
        var primaryKey = entityEntry.Metadata.FindPrimaryKey();

        if (primaryKey != null && primaryKey.Properties.Count == 1)
        {
            var property = primaryKey.Properties[0];
            var value = entityEntry.Property(property.Name).CurrentValue;
            return value is Guid guid ? guid : Guid.Parse(value?.ToString() ?? Guid.Empty.ToString());
        }

        throw new InvalidOperationException($"Cannot get ID for entity of type {typeof(T).Name}");
    }
}