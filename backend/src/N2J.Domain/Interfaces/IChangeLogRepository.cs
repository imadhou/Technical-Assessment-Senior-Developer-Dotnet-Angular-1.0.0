using N2J.Domain.Entities;

namespace N2J.Domain.Interfaces;

public interface IChangeLogRepository
{
    Task AddAsync(ChangeLog changeLog);
    Task<IEnumerable<ChangeLog>> GetAllByEntityIdAsync(Guid entityId);

}
