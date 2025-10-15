using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using N2J.Infrastructure.Persistence.Audit;

namespace N2J.Infrastructure.Persistence.Extensions
{
    public static class ChangeTrackingExtensions
    {
        public static Dictionary<string, object> GetPropertyChanges<T>(this EntityEntry<T> entityEntry) where T : class
        {
            var changes = new Dictionary<string, object>();

            foreach (var property in entityEntry.Properties)
            {
                if (property.Metadata.IsPrimaryKey() || IsAuditProperty(property.Metadata.Name))
                    continue;

                if (property.IsModified || entityEntry.State == EntityState.Added || entityEntry.State == EntityState.Deleted)
                {
                    var changeInfo = new
                    {
                        OldValue = entityEntry.State == EntityState.Added ? null : property.OriginalValue,
                        NewValue = entityEntry.State == EntityState.Deleted ? null : property.CurrentValue,
                        Type = property.Metadata.ClrType.Name
                    };

                    changes[property.Metadata.Name] = changeInfo;
                }
            }

            return changes;
        }

        public static async Task LogChangeAsync<T>(
            this IChangeLogRepository changeLogRepository,
            T entity,
            string changeType,
            Guid changedBy,
            EntityEntry<T> entityEntry) where T : class
        {
            var changes = entityEntry.GetPropertyChanges();

            await changeLogRepository.LogChangeAsync(
                entity: entity,
                changeType: changeType,
                changedBy: changedBy,
                changes: changes
            );
        }

        private static bool IsAuditProperty(string propertyName)
        {
            var auditProperties = new[] { "CreatedAt", "UpdatedAt", "PasswordHash" };
            return auditProperties.Contains(propertyName);
        }
    }
}
