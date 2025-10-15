using N2J.Application.Models.Dtos;

namespace N2J.Application.Interfaces;

public interface IChangeLogService
{
    Task AddChangeLogAsync(Guid entityId, string entityType, string changeType, string changedBy, string changeDescription);
    Task<ICollection<string>> GetChangeLogsByEntityIdAsync(Guid entityId);
}
