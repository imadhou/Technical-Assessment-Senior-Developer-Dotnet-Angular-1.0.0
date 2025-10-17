using N2J.Domain.Entities;
namespace N2J.Domain.Interfaces;

public interface IUserRepository: IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetFullByIdAsync(Guid id);
    Task<bool> ExistsByEmail(string email);

}
