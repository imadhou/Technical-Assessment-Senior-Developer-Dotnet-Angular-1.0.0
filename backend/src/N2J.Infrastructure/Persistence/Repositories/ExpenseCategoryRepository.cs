using N2J.Domain.Entities;
using N2J.Domain.Interfaces;
using N2J.Infrastructure.Persistence.Audit;

namespace N2J.Infrastructure.Persistence.Repositories
{
    public class ExpenseCategoryRepository: Repository<ExpenseCategory>, IExpenceCategoryRepository
    {
        public ExpenseCategoryRepository(AppDbContext context, IChangeLogRepository changeLogRepository) : base(context, changeLogRepository)
        {
        }
    }
}
