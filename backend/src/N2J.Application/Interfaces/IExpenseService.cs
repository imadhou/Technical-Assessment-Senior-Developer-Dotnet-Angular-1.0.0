using N2J.Application.Models.Dtos;
using N2J.Application.Models.Queries;

namespace N2J.Application.Interfaces;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseDto>> GetAll(ExpenseQuery query);
    Task<ExpenseDto?> GetById(Guid id);
}
