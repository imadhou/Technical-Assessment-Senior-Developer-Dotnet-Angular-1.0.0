using N2J.Domain.Entities;

namespace N2J.Domain.Interfaces;

public interface IRoleRepository: IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
}
