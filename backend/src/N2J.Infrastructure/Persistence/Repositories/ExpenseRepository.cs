using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using N2J.Domain.Entities;
using N2J.Domain.Interfaces;
using N2J.Infrastructure.Persistence.Audit;

namespace N2J.Infrastructure.Persistence.Repositories;

public class ExpenseRepository : Repository<Expense>, IExpenseRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Expense> _dbSet;

    public ExpenseRepository(AppDbContext context, IChangeLogRepository changeLogRepository) : base(context, changeLogRepository)
    {
        _context = context;
        _dbSet = _context.Set<Expense>();
    }
    
    public async Task<Expense?> GetByIdAsync(Guid id, params Expression<Func<Expense, object>>[] includes)
    {
        IQueryable<Expense> query = _dbSet.AsQueryable();

        foreach (var include in includes)
            query = query.Include(include);

        return await query.AsTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<ICollection<Expense>> GetAllAsync(
        ICollection<Expression<Func<Expense, bool>>> predicates,
        Expression<Func<Expense, object>> orderBy,
        int take,
        int skip,
        params Expression<Func<Expense, object>>[] includes)
    {
        IQueryable<Expense> query = _dbSet.AsQueryable();
        foreach (var include in includes)
            query = query.Include(include);
        foreach (var predicate in predicates)
            query = query.Where(predicate);
        if (orderBy != null)
            query = query.OrderBy(orderBy);
        if (skip > 0)
            query = query.Skip(skip);
        if (take > 0)
            query = query.Take(take);
        List<Expense> expenses = await query.AsNoTracking().ToListAsync();
        return expenses;
    }

    public async Task<(ICollection<Expense> Items, int TotalCount)> GetAllWithCountAsync(
        ICollection<Expression<Func<Expense, bool>>> predicates,
        Expression<Func<Expense, object>> orderBy,
        bool isDescending,
        int take,
        int skip,
        params Expression<Func<Expense, object>>[] includes)
    {
        IQueryable<Expense> query = _dbSet.AsQueryable();
        foreach (var include in includes)
            query = query.Include(include);
        foreach (var predicate in predicates)
            query = query.Where(predicate);
        if (orderBy != null && isDescending)
            query = query.OrderByDescending(orderBy);
        if (orderBy != null && !isDescending)
            query = query.OrderBy(orderBy);
        if (skip > 0)
            query = query.Skip(skip);
        if (take > 0)
            query = query.Take(take);
        var totalCount = await query.CountAsync();
        List<Expense> expenses = await query.AsNoTracking().ToListAsync();
        return (expenses, totalCount);
    }
}
