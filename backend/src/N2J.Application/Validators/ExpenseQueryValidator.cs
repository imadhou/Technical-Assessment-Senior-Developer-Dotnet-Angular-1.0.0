using FluentValidation;
using N2J.Application.Models.Queries;

namespace N2J.Application.Validators;

/// <summary>
/// This class uses FluentValidation to validate the properties of the ExpenseQuery model.
/// </summary>
public class ExpenseQueryValidator : AbstractValidator<ExpenseQuery>
{
    public ExpenseQueryValidator()
    {
        // Pagination validation
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        // Description validation
        RuleFor(x => x.Description)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description filter cannot exceed 100 characters");

        // Amount validation
        RuleFor(x => x.MinAmount)
            .GreaterThanOrEqualTo(0).When(x => x.MinAmount.HasValue)
            .WithMessage("MinAmount must be greater than or equal to 0");

        RuleFor(x => x.MaxAmount)
            .GreaterThanOrEqualTo(0).When(x => x.MaxAmount.HasValue)
            .WithMessage("MaxAmount must be greater than or equal to 0");

        RuleFor(x => x)
            .Must(x => !x.MaxAmount.HasValue || !x.MinAmount.HasValue || x.MaxAmount >= x.MinAmount)
            .WithMessage("MaxAmount must be greater than or equal to MinAmount");

        // Date validation
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(DateTime.Today).When(x => x.FromDate.HasValue)
            .WithMessage("FromDate cannot be in the future");

        RuleFor(x => x.ToDate)
            .LessThanOrEqualTo(DateTime.Today).When(x => x.ToDate.HasValue)
            .WithMessage("ToDate cannot be in the future");

        RuleFor(x => x)
            .Must(x => !x.ToDate.HasValue || !x.FromDate.HasValue || x.ToDate >= x.FromDate)
            .WithMessage("ToDate must be greater than or equal to FromDate");

        // SortBy validation
        RuleFor(x => x.SortBy)
            .Must(BeAValidSortProperty)
            .When(x => !string.IsNullOrEmpty(x.SortBy))
            .WithMessage("SortBy must be one of: Date, Amount, Description, Category, CreatedAt");
    }

    private static bool BeAValidSortProperty(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return true;

        var validProperties = new[] { "Date", "Amount", "Description", "Category", "CreatedAt" };
        return validProperties.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }
}