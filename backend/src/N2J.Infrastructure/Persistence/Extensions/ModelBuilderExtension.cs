using Microsoft.EntityFrameworkCore;
using N2J.Domain.Entities;

namespace N2J.Infrastructure.Persistence.Extensions;

/// <summary>
/// Provides extension methods for seeding data into the database using the <see cref="ModelBuilder"/>.
/// </summary>
/// <remarks>
/// This class contains a method to seed predefined data for entities such as companies, roles, users,
/// expense categories, and expenses. The seeded data includes static values for dates and identifiers to ensure
/// consistency across different environments. This method is typically used during database initialization or
/// migrations to populate the database with initial data.</remarks>
public static class ModelBuilderExtension
{
    // Define static, hardcoded dates
    private static readonly DateTime _seedDate = new DateTime(2025, 10, 12, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime _expenseDate1 = new DateTime(2025, 10, 2, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime _expenseDate2 = new DateTime(2025, 10, 7, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime _expenseDate3 = new DateTime(2025, 10, 10, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Seeds the database with initial data for entities such as companies, roles, users, expense categories, and
    /// expenses.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> instance used to configure the entity data to be seeded.</param>
    public static void Seed(this ModelBuilder modelBuilder)
    {
        // Companies
        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "N2JSoft",
                Address = "1 Rue de la Paix, 75002 Paris, France",
                Currency = "EUR",
                CreatedAt = _seedDate
            },
            new Company
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Tech Solutions",
                Address = "123 Tech Avenue, San Francisco, CA, USA",
                Currency = "USD",
                CreatedAt = _seedDate
            }
        );

        // Roles
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "Admin",
                Description = "Administrator with full access"
            },
            new Role
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Manager",
                Description = "Manager with limited access"
            },
            new Role
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Name = "Employee",
                Description = "Employee with basic access"
            }
        );

        // Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Email = "charles.leclerc@n2j.fr",
                FirstName = "Charles",
                LastName = "Leclerc",
                CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                RoleId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                CreatedAt = _seedDate
            },
            new User
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Email = "max.verstappen@n2j.fr",
                FirstName = "Max",
                LastName = "Verstappen",
                CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                RoleId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                CreatedAt = _seedDate
            },
            new User
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Email = "oscar.piastri@n2j.fr",
                FirstName = "Oscar",
                LastName = "Piastri",
                CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                RoleId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                CreatedAt = _seedDate
            },
            new User
            {
                Id = Guid.Parse("11111111-aaaa-1111-aaaa-111111111111"),
                Email = "lando.norris@techsol.us",
                FirstName = "Lando",
                LastName = "Norris",
                CompanyId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                RoleId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                CreatedAt = _seedDate
            },
            new User
            {
                Id = Guid.Parse("22222222-bbbb-2222-bbbb-222222222222"),
                Email = "george.russell@techsol.us",
                FirstName = "George",
                LastName = "Russell",
                CompanyId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                RoleId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                CreatedAt = _seedDate
            }
        );

        // Expense Categories
        modelBuilder.Entity<ExpenseCategory>().HasData(
            new ExpenseCategory
            {
                Id = Guid.Parse("aaaaaaaa-1111-aaaa-1111-aaaaaaaa1111"),
                Name = "Travel",
                Description = "Expenses related to travel",
                CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = _seedDate
            },
            new ExpenseCategory
            {
                Id = Guid.Parse("bbbbbbbb-2222-bbbb-2222-bbbbbbbb2222"),
                Name = "Meals",
                Description = "Expenses for meals and entertainment",
                CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = _seedDate
            },
            new ExpenseCategory
            {
                Id = Guid.Parse("cccccccc-3333-cccc-3333-cccccccc3333"),
                Name = "Office Supplies",
                Description = "Expenses for office supplies and equipment",
                CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = _seedDate
            }
        );

        // Expenses
        modelBuilder.Entity<Expense>().HasData(
            new Expense
            {
                Id = Guid.Parse("aaaa1111-aaaa-1111-aaaa-aaaaaaaaaaaa"),
                Title = "Flight to Conference",
                Description = "Flight ticket to attend the tech conference",
                Amount = 500m,
                Date = _expenseDate1,
                UserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                ExpenseCategoryId = Guid.Parse("aaaaaaaa-1111-aaaa-1111-aaaaaaaa1111"),
                CreatedAt = _expenseDate1
            },
            new Expense
            {
                Id = Guid.Parse("bbbb2222-bbbb-2222-bbbb-bbbbbbbbbbbb"),
                Title = "Team Lunch",
                Description = "Lunch with the team to discuss project updates",
                Amount = 150m,
                Date = _expenseDate2,
                UserId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                ExpenseCategoryId = Guid.Parse("bbbbbbbb-2222-bbbb-2222-bbbbbbbb2222"),
                CreatedAt = _expenseDate2 
            },
            new Expense
            {
                Id = Guid.Parse("cccc3333-cccc-3333-cccc-cccccccccccc"),
                Title = "Office Chair",
                Description = "Ergonomic office chair for better comfort",
                Amount = 300m,
                Date = _expenseDate3,
                UserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                ExpenseCategoryId = Guid.Parse("cccccccc-3333-cccc-3333-cccccccc3333"),
                CreatedAt = _expenseDate3
            }
        );
    }
}