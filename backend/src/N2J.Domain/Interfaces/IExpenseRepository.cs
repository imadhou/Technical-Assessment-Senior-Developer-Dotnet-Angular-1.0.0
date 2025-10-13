using N2J.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace N2J.Domain.Interfaces;

public interface IExpenseRepository: IRepository<Expense>
{
    Task<Expense?> GetByIdAsync(Guid id, params Expression<Func<Expense, object>>[] includes);
    Task<IEnumerable<Expense>> GetAllAsync(IEnumerable<Expression<Func<Expense, bool>>> predicates, Expression<Func<Expense, object>> orderBy, int take, int skip, params Expression<Func<Expense, object>>[] includes);
}