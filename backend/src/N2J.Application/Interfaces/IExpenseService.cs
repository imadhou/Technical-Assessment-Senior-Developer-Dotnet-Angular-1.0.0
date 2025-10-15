using N2J.Application.Models.Commands;
using N2J.Application.Models.Dtos;
using N2J.Application.Models.Queries;

namespace N2J.Application.Interfaces;

public interface IExpenseService
{
    Task<ICollection<ExpenseDto>> GetAllAsync(ExpenseQuery query);
    Task<(ICollection<ExpenseDto> Items, int TotalCount)> GetAllWithCountAsync(ExpenseQuery query);
    Task<ExpenseDto?> GetByIdAsync(Guid id);
    Task<ExpenseDto> CreateExpenseAsync(ExpenseCommand expenseCommand, Guid createdBy);
    Task<ExpenseDto> UpdateExpenseAsync(Guid expenseId, ExpenseCommand expenseCommand, Guid updatedBy);
    Task<ExpenseDto> UpdateExpenseStatusAsync(Guid expenseId, string newStatus, Guid updatedBy);

}
