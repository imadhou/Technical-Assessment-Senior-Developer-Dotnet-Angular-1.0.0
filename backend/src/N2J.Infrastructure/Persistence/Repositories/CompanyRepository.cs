using Microsoft.EntityFrameworkCore;
using N2J.Domain.Entities;
using N2J.Domain.Interfaces;
using N2J.Infrastructure.Persistence.Audit;

namespace N2J.Infrastructure.Persistence.Repositories
{
    public class CompanyRepository: Repository<Company>, ICompanyRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Company> _dbSet;
        public CompanyRepository(AppDbContext context, IChangeLogRepository changeLogRepository) : base(context, changeLogRepository)
        {
            _context = context;
            _dbSet = _context.Set<Company>();
        }

        public async Task<Company?> GetByNameAsync(string domain)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name.ToLower() == domain.ToLower());
        }
    }
}
