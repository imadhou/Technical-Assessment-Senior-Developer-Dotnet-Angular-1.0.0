using System.Linq.Expressions;

namespace N2J.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity, Guid? createdBy=null, bool logChanges=false);
    Task UpdateAsync(TEntity entity, Guid? updatedBy=null, bool logChanges=false);
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<Expression<Func<TEntity, bool>>> predicates);
    Task DeleteAsync(TEntity entity, Guid? deletedBy = null, bool logChanges =false);
    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);
}
