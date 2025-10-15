using AutoMapper;
using N2J.Application.Interfaces;
using N2J.Application.Models.Commands;
using N2J.Application.Models.Dtos;
using N2J.Application.Models.Queries;
using N2J.Domain.Entities;
using N2J.Domain.Interfaces;
using System.Linq.Expressions;

namespace N2J.Application.Services;

public class ExpenseService: IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IMapper _mapper;

    public ExpenseService(IExpenseRepository expenseRepository, IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _mapper = mapper;
    }

    public async Task<ExpenseDto> UpdateExpenseStatus(Guid expenseId, string newStatus, Guid updatedBy)
    {
        var expense = await _expenseRepository.GetByIdAsync(expenseId);
        if (expense == null)
        {
            throw new KeyNotFoundException($"Expense with ID {expenseId} not found.");
        }

        expense.Status = newStatus;
        expense.UpdatedAt = DateTime.UtcNow;
        expense.UpdatedBy = updatedBy;

        await _expenseRepository.UpdateAsync(expense, updatedBy, logChanges: true);
        return _mapper.Map<ExpenseDto>(expense);
    }

    public async Task<ExpenseDto> UpdateExpense(Guid expenseId, ExpenseCommand expenseCommand, Guid updatedBy)
    {
        var expense = await _expenseRepository.GetByIdAsync(expenseId);

        if (expense == null)
        {
            throw new KeyNotFoundException($"Expense with ID {expenseId} not found.");
        }

        expense = _mapper.Map(expenseCommand, expense);
        expense.UpdatedAt = DateTime.UtcNow;
        expense.UpdatedBy = updatedBy;

        await _expenseRepository.UpdateAsync(expense, updatedBy, logChanges: true);
        return _mapper.Map<ExpenseDto>(expense);
    }

    public async Task<ExpenseDto> CreateExpense(ExpenseCommand expenseCommand, Guid createdBy)
    {
        expenseCommand.UserId = expenseCommand.UserId ?? createdBy;
        Expense expense = _mapper.Map<Expense>(expenseCommand);
        expense.CreatedBy = createdBy;
        expense.CreatedAt = DateTime.UtcNow;

        await _expenseRepository.AddAsync(expense, createdBy, logChanges: true);

        return _mapper.Map<ExpenseDto>(expense);
    }



    public async Task<IEnumerable<ExpenseDto>> GetAll(ExpenseQuery query)
    {
        var predicates = BuildPredicates(query);
        var orderBy = GetOrderByExpression(query);

        var expenses = await _expenseRepository.GetAllAsync(
            predicates: predicates,
            orderBy: orderBy,
            take: query.PageSize,
            skip: (query.Page - 1) * query.PageSize,
            e => e.ExpenseCategory,
            e => e.User
        );
        var expenseDtos = _mapper.Map<IEnumerable<ExpenseDto>>(expenses);

        return expenseDtos;
    }

    public async Task<ExpenseDto?> GetById(Guid id)
    { 
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense == null) return null;
        var expenseDto = _mapper.Map<ExpenseDto>(expense);
        return expenseDto;
    }

    private IEnumerable<Expression<Func<Expense, bool>>> BuildPredicates(ExpenseQuery query)
    {
        var predicates = new List<Expression<Func<Expense, bool>>>();

        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            predicates.Add(e => e.Description.Contains(query.Description));
        }

        if (query.ExpenseCategoryId.HasValue)
        {
            predicates.Add(e => e.ExpenseCategoryId == query.ExpenseCategoryId);
        }

        if (query.MinAmount.HasValue)
        {
            predicates.Add(e => e.Amount >= query.MinAmount.Value);
        }

        if (query.MaxAmount.HasValue)
        {
            predicates.Add(e => e.Amount <= query.MaxAmount.Value);
        }

        if (query.FromDate.HasValue)
        {
            predicates.Add(e => e.Date >= query.FromDate.Value.Date);
        }

        if (query.ToDate.HasValue)
        {
            predicates.Add(e => e.Date <= query.ToDate.Value.Date);
        }

        return predicates;
    }

    private Expression<Func<Expense, object>> GetOrderByExpression(ExpenseQuery query)
    {
        return query.SortBy?.ToLower() switch
        {
            "amount" => e => e.Amount,
            "date" => e => e.Date,
            "description" => e => e.Description,
            "expenseCategoryId" => e => e.ExpenseCategoryId,
            "createdat" => e => e.CreatedAt,
            _ => e => e.CreatedAt // Default
        };
    }
}
