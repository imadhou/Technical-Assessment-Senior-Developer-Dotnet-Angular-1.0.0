using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using N2J.Domain.Interfaces;
using N2J.Infrastructure.Persistence.Audit;

namespace N2J.Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly IChangeLogRepository _changeLogRepository;

    public Repository(AppDbContext context, IChangeLogRepository changeLogRepository)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
        _changeLogRepository = changeLogRepository;
    }

    public async Task AddAsync(TEntity entity, Guid? createdBy = null, bool logChanges = false)
    {
        await _dbSet.AddAsync(entity);

        if(logChanges)
        {
            var entry = _context.Entry(entity);
            await _changeLogRepository.LogChangeWithTrackingAsync(
                entity: entity,
                changeType: "CREATE",
                changedBy: createdBy,
                entityEntry: entry
            );
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity, Guid? deletedBy = null, bool logChanges = false)
    {
        _dbSet.Remove(entity);

        if (logChanges)
        {
            var entry = _context.Entry(entity);
            await _changeLogRepository.LogChangeWithTrackingAsync(
                entity: entity,
                changeType: "DELETE",
                changedBy: deletedBy,
                entityEntry: entry
            );
        }
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<ICollection<TEntity>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<ICollection<TEntity>> GetAllAsync(ICollection<Expression<Func<TEntity, bool>>> predicates)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        foreach (var predicate in predicates)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task UpdateAsync(TEntity entity, Guid? updatedBy = null, bool logChanges = false)
    {
        _dbSet.Update(entity);

        if (logChanges)
        {
            var entry = _context.Entry(entity);
            await _changeLogRepository.LogChangeWithTrackingAsync(
                entity: entity,
                changeType: "UPDATE",
                changedBy: updatedBy,
                entityEntry: entry
            );
        }
        await _context.SaveChangesAsync();
    }
}
