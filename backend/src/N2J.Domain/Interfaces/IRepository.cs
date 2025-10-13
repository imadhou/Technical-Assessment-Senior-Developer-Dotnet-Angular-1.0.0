using System.Linq.Expressions;

namespace N2J.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<Expression<Func<TEntity, bool>>> predicates);
    void Delete(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);
}
