using N2J.Application.Models.Commands;
using N2J.Application.Models.Dtos;
using N2J.Application.Models.Queries;

namespace N2J.Application.Interfaces;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseDto>> GetAll(ExpenseQuery query);
    Task<ExpenseDto?> GetById(Guid id);
    Task<ExpenseDto> CreateExpense(ExpenseCommand expenseCommand, Guid createdBy);
    Task<ExpenseDto> UpdateExpense(Guid expenseId, ExpenseCommand expenseCommand, Guid updatedBy);
    Task<ExpenseDto> UpdateExpenseStatus(Guid expenseId, string newStatus, Guid updatedBy);

}
