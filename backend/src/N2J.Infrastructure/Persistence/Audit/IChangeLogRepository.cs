using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace N2J.Infrastructure.Persistence.Audit;

public interface IChangeLogRepository
{
    Task LogChangeAsync<T>(T entity, string changeType, Guid? changedBy, Dictionary<string, object>? changes = null) where T : class;
    Task<List<ChangeLog>> GetEntityHistoryAsync(string entityName, Guid entityId, int? limit = null);
    Guid GetEntityId<T>(T entity) where T : class;
    Task LogChangeWithTrackingAsync<T>(T entity, string changeType, Guid? changedBy, EntityEntry<T> entityEntry) where T : class;

}
