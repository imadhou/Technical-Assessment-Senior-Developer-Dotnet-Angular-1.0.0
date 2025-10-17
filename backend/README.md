# Expense Management System - Backend Documentation
## Key Business Features
### 1. Expense Management
Complete Expense Lifecycle
Create Expenses: Employees can submit expense claims with details (title, amount, category, date, description)

Update Expenses: Modify expense details before approval

Status Tracking: Real-time status updates (Pending → Approved/Rejected)

Bulk Operations: Efficient handling of multiple expenses

Search & Filtering: Advanced filtering by status, category, date range, amount, user

Expense Categories
Pre-defined categories per company (Office Supplies, Travel, Meals, etc.)

Company-specific categorization

Flexible category management

### 2. Receipt Management
Digital Receipt Handling
File Upload: Support for PDF, JPG, PNG formats (up to 10MB)

Secure Storage: Azure Blob Storage with organized folder structure

Access Control: Role-based receipt access (employees see own, managers see team)

Automatic Cleanup: Receipt deletion when expenses are deleted

File Validation
Size limits enforcement

File type verification

Virus scanning readiness

### 3. Authentication & Authorization
User Authentication
JWT-based Authentication: Secure token-based authentication

Password Security: PBKDF2 hashing with 100,000 iterations

Session Management: Configurable token expiration

User Registration: Self-service registration with company assignment

Role-Based Access Control
Employee Role:

Create/edit/delete own expenses

Upload/view own receipts

View personal expense history

Manager Role:

All employee permissions

Approve/reject team expenses

View all team member expenses

Generate team reports

### 4. Multi-tenant Architecture
Company Isolation
Data Segregation: Complete separation between companies

Custom Configuration: Per-company expense categories

User Management: Company-specific user bases

Reporting: Company-level analytics and reporting

### 5. Audit & Compliance
Complete Audit Trail
Change Logging: Every create, update, delete operation tracked

User Attribution: Who made each change

Before/After Values: Property-level change tracking

Compliance Ready: Meets regulatory requirements for financial tracking

## 🏗️ System Architecture
````plaintext
N2J.EXPENSE MANAGEMENT SYSTEM/
├── AppHost/
├── WebApi/
│   ├── Controllers/
│   ├── Startup/
│   ├── Middleware/
│   ├── Extensions/
│   ├── Attributes/
│   └── Models/
├── Application/
│   ├── Services/
│   ├── Interfaces/
│   ├── Models/
│   │   ├── Dtos/
│   │   │   ├── Expense/
│   │   │   ├── UserAuth/
│   │   │   └── Company/
│   │   ├── Commands/
│   │   │   ├── Expense/
│   │   │   ├── UserAuth/
│   │   │   └── FileUpload/
│   │   ├── Queries/
│   │   └── Configs/
│   ├── Mappers/
│   ├── Validators/
│   ├── Exceptions/
│   └── Constants/
├── Domain/
│   ├── Entities/
│   ├── Interfaces/
│   ├── ValueObjects/
│   ├── Enums/
│   └── Events/
├── Infrastructure/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   ├── Repositories/
│   │   ├── Migrations/
│   │   └── Audit/
│   ├── DataSeeding/
│   ├── Services/
│   ├── Extensions/
````

### Domain-Driven Design (DDD) Implementation
Core Domain Entities
````csharp
// Expense Aggregate Root
public class Expense : BaseEntity
{
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public Guid UserId { get; set; }
    public Guid ExpenseCategoryId { get; set; }
    public Guid CompanyId { get; set; }
}

// User Aggregate Root
public class User : BaseEntity
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public Guid CompanyId { get; set; }
    public Guid RoleId { get; set; }
}
````

## Clean Architecture Layers
### 1. Domain Layer (Core Business Logic)
Entities: Core business objects with behavior

Value Objects: Immutable objects without identity

Domain Services: Domain-specific business rules

Repository Interfaces: Data access contracts

### 2. Application Layer (Use Cases)
DTOs: Data transfer objects for API contracts

Commands & Queries: CQRS pattern implementation

Application Services: Orchestrate domain objects for use cases

Validators: Business rule validation

### 3. Infrastructure Layer (External Concerns)
Data Persistence: Entity Framework with PostgreSQL

External Services: Redis, Azure Blob Storage, Email

Repository Implementations: Data access concrete classes

### 4. Presentation Layer (Web API)
Controllers: RESTful API endpoints

Middleware: Cross-cutting concerns

DTO Mapping: Entity to API contract transformation

Dependency Injection & IoC Container
Service Registration
````csharp
// Application Services
services.AddScoped<IExpenseService, ExpenseService>();
services.AddScoped<IUserAuthService, UserAuthService>();

// Domain Services
services.AddScoped<IExpenseRepository, ExpenseRepository>();
services.AddScoped<IUserRepository, UserRepository>();

// Infrastructure Services
services.AddSingleton<IRedisCacheService, RedisCacheService>();
services.AddScoped<IFileStorageService, FileStorageService>();

// Cross-cutting Concerns
services.AddTransient<ExceptionHandlingMiddleware>();
services.AddTransient<RequestLoggingMiddleware>();
````

## 🔧 Technical Key Features
### 1. Advanced Caching System
Multi-level Caching Strategy
```` csharp
// Intelligent cache key generation
public string BuildCacheKey(ExpenseQuery query, Guid companyId)
{
    // Skip cache for complex queries
    bool hasUncachedFilters = !string.IsNullOrEmpty(query.Description) 
        || query.FromDate.HasValue 
        || query.ToDate.HasValue;
    
    if (hasUncachedFilters) return null;
    
    // Generate specific cache keys based on query parameters
    return CacheKeys.ExpensesByCompanyAndCategory(
        companyId, query.ExpenseCategoryId.Value, 
        query.Page, query.PageSize, query.SortBy, query.IsDescending);
}
````
Cache Group Management
````csharp
// Group cache keys by company for bulk invalidation
private async Task InvalidateCompanyCaches(Guid companyId)
{
    var groupKey = GetCompanyGroupKey(companyId);
    await _cacheService.RemoveGroupAsync(groupKey);
}
````

### 2. Security Implementation
JWT Token Security
````csharp
var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, roleName),
        new Claim("companyId", user.CompanyId.ToString()),
        new Claim("firstName", user.FirstName),
        new Claim("lastName", user.LastName)
    }),
    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
    Issuer = _jwtSettings.Issuer,
    Audience = _jwtSettings.Audience,
    SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key), 
        SecurityAlgorithms.HmacSha256Signature)
};
````
### 3. Validation & Error Handling
FluentValidation Integration

````csharp
public class ExpenseCommandValidator : AbstractValidator<ExpenseCommand>
{
    public ExpenseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");
        
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");
        
        RuleFor(x => x.ExpenseCategoryId)
            .NotEmpty().WithMessage("Expense category is required");
    }
}
````
Global Exception Handling

````csharp
public class ExceptionHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Structured error response
        var problemDetails = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = "An error occurred while processing your request",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Instance = context.Request.Path
        };
        
        // Development vs Production error details
        if (_environment.IsDevelopment())
        {
            problemDetails.Detail = exception.ToString();
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }
    }
}
````
### 4. API Design & Versioning -RESTful API with HATEOAS-
````csharp
public class ExpenseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public List<LinkDto> Links { get; set; } // Hypermedia controls
}

// Link generation for HATEOAS
private List<LinkDto> GenerateExpenseLinks(Guid expenseId, string status, UserContext userContext)
{
    var links = new List<LinkDto>();
    
    // Self link
    links.Add(new LinkDto($"/api/v1/expenses/{expenseId}", "self", "GET"));
    
    // Conditional links based on user role and expense status
    if (userContext.Role == AppConstants.ManagerRole && status == "Pending")
    {
        links.Add(new LinkDto($"/api/v1/expenses/{expenseId}/status", "approve", "PATCH"));
    }
    
    return links;
}
````

### 4- API Versioning
```` csharp
services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1.0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
````
### 5. Performance & Scalability
Database Optimization

````csharp
public async Task<(ICollection<Expense> Items, int TotalCount)> GetAllWithCountAsync(
    ICollection<Expression<Func<Expense, bool>>> predicates = null,
    Expression<Func<Expense, object>> orderBy = null,
    bool isDescending = false,
    int take = 20,
    int skip = 0,
    params Expression<Func<Expense, object>>[] includes)
{
    // Efficient query building with pagination at database level
    IQueryable<Expense> query = _dbSet.AsQueryable();
    
    // Apply predicates
    if (predicates != null)
    {
        foreach (var predicate in predicates)
        {
            query = query.Where(predicate);
        }
    }
    
    // Apply ordering
    if (orderBy != null)
    {
        query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
    }
    
    // Get total count before pagination
    var totalCount = await query.CountAsync();
    
    // Apply pagination and includes
    var items = await query
        .Skip(skip)
        .Take(take)
        .IncludeMultiple(includes)
        .ToListAsync();
        
    return (items, totalCount);
}
````
### 1. Observability & Monitoring
OpenTelemetry Integration

````csharp
services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddRuntimeInstrumentation()
               .AddOtlpExporter();
    })
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddOtlpExporter();
    });
````
Structured Logging

````csharp
public class RequestLoggingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        
        _logger.LogInformation("HTTP {Method} {Path} started", 
            context.Request.Method, context.Request.Path);
        
        await _next(context);
        
        var elapsed = DateTime.UtcNow - startTime;
        
        _logger.LogInformation("HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMs}ms",
            context.Request.Method, context.Request.Path, 
            context.Response.StatusCode, elapsed.TotalMilliseconds);
    }
}
````

### File Management - Azure Blob Storage Integration
````csharp
public class ExpenseReceiptService : IExpenseReceiptService
{
    public async Task<ReceiptDto> UploadReceiptAsync(FileUploadCommand request, Guid userId)
    {
        // File validation
        ValidateFile(request.File);
        
        // Generate secure file path
        var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        var fileName = $"/user/{userId}/expenses/{request.ExpenseId}/{Guid.NewGuid()}{fileExtension}";
        
        // Upload to blob storage
        using var stream = request.File.OpenReadStream();
        var fileUrl = await _fileStorageService.UploadFileAsync(stream, fileName, request.File.ContentType);
        
        return new ReceiptDto
        {
            FileName = request.File.FileName,
            FileUrl = fileUrl,
            FileSize = request.File.Length,
            ContentType = request.File.ContentType,
            UploadedAt = DateTime.UtcNow,
        };
    }
}
````

### 1. Testing & Data Seeding
Comprehensive Test Data

````csharp
public class DataSeeder
{
    public async Task SeedAsync()
    {
        // Create 10 companies with realistic data
        // 2 managers + 13 employees per company
        // 0-15 expenses per user with realistic status distribution
        // Realistic expense categories per company
    }
}
````

This backend architecture provides a robust, scalable, and maintainable foundation for the expense management system, incorporating modern software engineering practices and enterprise-grade patterns while ensuring security, performance, and developer productivity.