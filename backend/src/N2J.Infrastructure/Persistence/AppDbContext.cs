using Microsoft.EntityFrameworkCore;
using N2J.Domain.Entities;
using N2J.Infrastructure.Persistence.Audit;
using N2J.Infrastructure.Persistence.Configurations;
using N2J.Infrastructure.Persistence.DateTimeConverters;
using N2J.Infrastructure.Persistence.Extensions;

namespace N2J.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<ChangeLog> ChangeLogs => Set<ChangeLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExpenseEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ExpenseCategoryEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ChangeLogEntityTypeConfiguration());

        modelBuilder.Seed();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        /// Use UTC converters for all DateTime and DateTime? properties
        configurationBuilder.Properties<DateTime>()
            .HaveConversion<UtcConverter>();
        configurationBuilder.Properties<DateTime?>()
            .HaveConversion<NullableUtcConverter>();
    }
}