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
    public class UserRepository: Repository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<User> _dbSet;
        public UserRepository(AppDbContext context, IChangeLogRepository changeLogRepository) : base(context, changeLogRepository)
        {
            _context = context;
            _dbSet = _context.Set<User>();
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<bool> ExistsByEmail(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }
    }
}
