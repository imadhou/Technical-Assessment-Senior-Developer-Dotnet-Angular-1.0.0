using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using N2J.Domain.Entities;
using N2J.Infrastructure.Persistence;

namespace N2J.Infrastructure;

public class DataSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DataSeeder> _logger;
    private readonly Faker _faker;
    private readonly string _managerRole = "Manager";
    private readonly string _employeeRole = "Employee";
    private readonly string _defaultPassword = "hqfaJDZnZzi4z1CNs/RV5w==.uAOO34uX3thcv1mCvlQNXqoOazlv/GX44/93vaAYf8k="; // Hashed "Password123!"

    public DataSeeder(AppDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
        _faker = new Faker();
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting data seeding...");

            // Check if data already exists
            if (await _context.Companies.AnyAsync())
            {
                _logger.LogInformation("Data already seeded. Skipping...");
                return;
            }

            // Create roles first
            var roles = await SeedRoles();

            // Create companies
            var companies = await SeedCompanies();

            // Create expense categories for each company
            var categories = await SeedExpenseCategories(companies);

            // Create users for each company
            var users = await SeedUsers(companies, roles);

            // Create expenses for each user
            await SeedExpenses(users, categories);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Data seeding completed successfully");
            _logger.LogWarning("All seeded users have password: 'Password123!'");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during data seeding");
            throw;
        }
    }

    private async Task<Dictionary<string, Guid>> SeedRoles()
    {
        var roles = new List<Role>
        {
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = _managerRole,
                Description = "Company Manager with administrative privileges",
            },
            new()
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = _employeeRole,
                Description = "Regular Employee",
            }
        };

        // Check if roles already exist to avoid duplicates
        var existingRoles = await _context.Roles.ToListAsync();
        var rolesToAdd = roles.Where(r => !existingRoles.Any(er => er.Name == r.Name)).ToList();

        if (rolesToAdd.Any())
        {
            await _context.Roles.AddRangeAsync(rolesToAdd);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created roles: {Roles}", string.Join(", ", rolesToAdd.Select(r => r.Name)));
        }
        else
        {
            _logger.LogInformation("Roles already exist");
        }

        // Return a dictionary for easy lookup
        return roles.ToDictionary(r => r.Name, r => r.Id);
    }

    private async Task<List<Company>> SeedCompanies()
    {
        var companies = new List<Company>
        {
            new() { Name = "TechCorp Solutions", Domain = "techcorp.com" },
            new() { Name = "Global Finance Inc", Domain = "globalfinance.com" },
            new() { Name = "HealthCare Partners", Domain = "healthcarepartners.com" },
            new() { Name = "EduSystems International", Domain = "edusystems.edu" },
            new() { Name = "Retail Giants Co", Domain = "retailgiants.com" },
            new() { Name = "Manufacturing Pros", Domain = "manufacturingpros.com" },
            new() { Name = "Consulting Experts Ltd", Domain = "consultingexperts.com" },
            new() { Name = "Logistics Masters", Domain = "logisticsmasters.com" },
            new() { Name = "Energy Solutions Co", Domain = "energysolutions.com" },
            new() { Name = "Creative Agency Inc", Domain = "creativeagency.com" },
        };

        await _context.Companies.AddRangeAsync(companies);
        await _context.SaveChangesAsync(); // Save to get IDs

        _logger.LogInformation("Created {Count} companies", companies.Count);
        return companies;
    }

    private async Task<Dictionary<Guid, List<ExpenseCategory>>> SeedExpenseCategories(List<Company> companies)
    {
        var allCategories = new Dictionary<Guid, List<ExpenseCategory>>();
        var baseCategories = new[]
        {
            "Travel & Accommodation",
            "Meals & Entertainment",
            "Office Supplies",
            "Software & Subscriptions",
            "Training & Development"
        };

        foreach (var company in companies)
        {
            var companyCategories = new List<ExpenseCategory>();

            foreach (var categoryName in baseCategories)
            {
                companyCategories.Add(new ExpenseCategory
                {
                    Name = categoryName,
                    Description = $"{categoryName} expenses for {company.Name}",
                    CompanyId = company.Id
                });
            }

            await _context.ExpenseCategories.AddRangeAsync(companyCategories);
            allCategories[company.Id] = companyCategories;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} expense categories across all companies", companies.Count * 5);

        return allCategories;
    }

    private async Task<List<User>> SeedUsers(List<Company> companies, Dictionary<string, Guid> roles)
    {
        var allUsers = new List<User>();

        foreach (var company in companies)
        {
            var companyUsers = new List<User>();

            // Create 2 managers per company
            for (int i = 1; i <= 2; i++)
            {
                var firstName = _faker.Name.FirstName();
                var lastName = _faker.Name.LastName();

                companyUsers.Add(new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = $"{firstName.ToLower()}.{lastName.ToLower()}@{company.Domain}",
                    PasswordHash = _defaultPassword,
                    CompanyId = company.Id,
                    RoleId = roles[_managerRole],
                    CreatedAt = DateTime.UtcNow.AddDays(-_faker.Random.Int(30, 365)),
                });
            }

            // Create 13 employees per company
            for (int i = 1; i <= 13; i++)
            {
                var firstName = _faker.Name.FirstName();
                var lastName = _faker.Name.LastName();

                companyUsers.Add(new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = $"{firstName.ToLower()}.{lastName.ToLower()}@{company.Domain}",
                    PasswordHash = _defaultPassword,
                    CompanyId = company.Id,
                    RoleId = roles[_employeeRole],
                    CreatedAt = DateTime.UtcNow.AddDays(-_faker.Random.Int(30, 365))
                });
            }

            allUsers.AddRange(companyUsers);

            _logger.LogInformation("Created {Count} users for company {Company}",
                companyUsers.Count, company.Name);
        }

        await _context.Users.AddRangeAsync(allUsers);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created {Count} users across all companies", allUsers.Count);

        // Log some sample user credentials for testing
        var sampleUsers = allUsers.Take(3).ToList();
        foreach (var user in sampleUsers)
        {
            _logger.LogInformation("Sample user: {Email} / Password123! (Role: {Role})",
                user.Email, user.RoleId == roles[_managerRole] ? "Manager" : "Employee");
        }

        return allUsers;
    }

    private async Task SeedExpenses(List<User> users, Dictionary<Guid, List<ExpenseCategory>> categoriesByCompany)
    {
        var allExpenses = new List<Expense>();
        var statuses = new[] { "Pending", "Approved", "Rejected" };
        var expenseDescriptions = new[]
        {
            "Business trip to client meeting",
            "Team lunch with stakeholders",
            "Office stationery purchase",
            "Software subscription renewal",
            "Professional development course",
            "Client entertainment dinner",
            "Hotel accommodation for conference",
            "Flight tickets for business travel",
            "Team building activity",
            "Work-from-home equipment",
            "Industry conference tickets",
            "Business books and materials",
            "Car rental for site visits",
            "Parking and toll expenses",
            "International roaming charges"
        };

        foreach (var user in users)
        {
            var expenseCount = _faker.Random.Int(0, 15); // 0 to 15 expenses per user
            var companyCategories = categoriesByCompany[user.CompanyId];

            for (int i = 0; i < expenseCount; i++)
            {
                var category = _faker.PickRandom(companyCategories);
                var amount = _faker.Random.Decimal(10, 1000);
                var expenseDate = DateTime.UtcNow.AddDays(-_faker.Random.Int(1, 90));
                var status = GetWeightedStatus(user, expenseDate);
                var description = _faker.PickRandom(expenseDescriptions);

                var expense = new Expense
                {
                    Description = description,
                    Title = description,
                    Amount = amount,
                    ExpenseCategoryId = category.Id,
                    UserId = user.Id,
                    Date = expenseDate,
                    Status = status,
                    ReceiptUrl = _faker.Random.Bool(0.7f) ? $"https://receipts.example.com/{_faker.Random.Guid()}.pdf" : null,
                    CreatedAt = expenseDate.AddHours(_faker.Random.Int(1, 8)),
                    UpdatedAt = status != "Pending" ? expenseDate.AddDays(_faker.Random.Int(1, 7)) : null
                };

                allExpenses.Add(expense);
            }
        }

        await _context.Expenses.AddRangeAsync(allExpenses);
        _logger.LogInformation("Created {Count} expenses across all users", allExpenses.Count);
    }

    private string GetWeightedStatus(User user, DateTime expenseDate)
    {
        // More recent expenses are more likely to be pending
        var daysAgo = (DateTime.UtcNow - expenseDate).Days;

        if (daysAgo <= 7)
        {
            return _faker.Random.WeightedRandom(
                ["Pending", "Approved", "Rejected"],
                [0.7f, 0.25f, 0.05f]
            );
        }
        else if (daysAgo <= 30)
        {
            return _faker.Random.WeightedRandom(
                ["Pending", "Approved", "Rejected"],
                [0.3f, 0.65f, 0.05f]
            );
        }
        else
        {
            return _faker.Random.WeightedRandom(
                ["Pending", "Approved", "Rejected"],
                [0.1f, 0.85f, 0.05f]
            );
        }
    }
}