using N2J.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace N2J.Domain.Interfaces;

public interface IExpenseRepository: IRepository<Expense>
{

    /// <summary>
    /// Retrieves an expense by its unique identifier, optionally including related entities.
    /// </summary>
    /// <param name="id">The unique identifier of the expense to retrieve.</param>
    /// <param name="includes">An array of expressions specifying related entities to include in the query result. Each expression represents a
    /// navigation property to include.</param>
    Task<Expense?> GetByIdAsync(Guid id, params Expression<Func<Expense, object>>[] includes);

    /// <summary>
    /// Retrieves a collection of <see cref="Expense"/> entities that match the specified criteria.
    /// </summary>
    /// <remarks>This method supports filtering, ordering, pagination, and eager loading of related entities. 
    /// If no predicates are provided, all entities are returned. If no includes are specified, only the primary entity
    /// data is retrieved.</remarks>
    /// <param name="predicates">A collection of predicate expressions used to filter the results. Each predicate is applied as a condition that
    /// the returned entities must satisfy.</param>
    /// <param name="orderBy">An expression specifying the property by which to order the results.</param>
    /// <param name="take">The maximum number of entities to retrieve. Must be a non-negative integer.</param>
    /// <param name="skip">The number of entities to skip before starting to retrieve results. Must be a non-negative integer.</param>
    /// <param name="includes">An optional array of expressions specifying related entities to include in the query results.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ICollection{T}"/> of
    /// <see cref="Expense"/> entities that match the specified criteria.</returns>
    Task<ICollection<Expense>> GetAllAsync(
        ICollection<Expression<Func<Expense, bool>>> predicates, 
        Expression<Func<Expense, object>> orderBy, 
        int take, 
        int skip, 
        params Expression<Func<Expense, object>>[] includes);

    /// <summary>
    /// Get All with totalCount
    /// </summary>
    /// <param name="predicates"></param>
    /// <param name="orderBy"></param>
    /// <param name="isDescending"></param>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<(ICollection<Expense> Items, int TotalCount)> GetAllWithCountAsync(
        ICollection<Expression<Func<Expense, bool>>> predicates,
        Expression<Func<Expense, object>> orderBy,
        bool isDescending,
        int take,
        int skip,
        params Expression<Func<Expense, object>>[] includes);

}