using Microsoft.EntityFrameworkCore;
using N2J.Domain.Entities;
using N2J.Domain.Interfaces;
using N2J.Infrastructure.Persistence.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Infrastructure.Persistence.Repositories
{
    public class RoleRepository: Repository<Role>, IRoleRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Role> _dbSet;
        public RoleRepository(AppDbContext context, IChangeLogRepository changeLogRepository) : base(context, changeLogRepository)
        {
            _context = context;
            _dbSet = _context.Set<Role>();
        }
        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}
