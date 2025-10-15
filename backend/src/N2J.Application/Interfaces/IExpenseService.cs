using N2J.Application.Models.Commands;
using N2J.Application.Models.Dtos;
using N2J.Application.Models.Queries;

namespace N2J.Application.Interfaces;

public interface IExpenseService
{
    Task<ICollection<ExpenseDto>> GetAllAsync(ExpenseQuery query, Guid companyId);
    Task<(ICollection<ExpenseDto> Items, int TotalCount)> GetAllWithCountAsync(ExpenseQuery query, Guid companyId);
    Task<ExpenseDto?> GetByIdAsync(Guid id);
    Task<ExpenseDto> CreateExpenseAsync(ExpenseCommand expenseCommand, Guid createdBy, Guid companyId);
    Task<ExpenseDto> UpdateExpenseAsync(Guid expenseId, ExpenseCommand expenseCommand, Guid updatedBy, Guid companyId);
    Task<ExpenseDto> UpdateExpenseStatusAsync(Guid expenseId, string newStatus, Guid updatedBy, Guid companyId  );

}
