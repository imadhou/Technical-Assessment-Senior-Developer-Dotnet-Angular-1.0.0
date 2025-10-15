using FluentValidation;
using N2J.Application.Models.Commands;

namespace N2J.Application.Validators;

/// <summary>
/// Validator for ExpenseCommand to ensure data integrity and business rules compliance
/// </summary>
public class ExpenseCommandValidator : AbstractValidator<ExpenseCommand>
{
    public ExpenseCommandValidator()
    {
        // Description validation
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(200).WithMessage("Description cannot exceed 200 characters")
            .MinimumLength(3).WithMessage("Description must be at least 3 characters")
            .Matches(@"^[a-zA-Z0-9\s\.\,\-\+\!\(\)]+$").WithMessage("Description contains invalid characters")
            .Must(BeAReasonableDescription).WithMessage("Description appears to be spam or invalid");

        // Amount validation
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0")
            .LessThanOrEqualTo(1000000).WithMessage("Amount cannot exceed 1,000,000")
            .PrecisionScale(18, 2, false).WithMessage("Amount cannot have more than 2 decimal places")
            .Must(BeAReasonableAmount).WithMessage("Amount appears to be unreasonable for an expense");

        // CategoryId validation
        RuleFor(x => x.ExpenseCategoryId)
            .NotEmpty().WithMessage("Category is required")
            .NotEqual(Guid.Empty).WithMessage("Category must be a valid selection");

        // Date validation
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required")
            .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Expense date cannot be in the future")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date.AddYears(-2)).WithMessage("Expense date cannot be more than 2 years in the past")
            .Must(BeAValidBusinessDate).WithMessage("Expense date falls on a weekend or holiday");

        // ReceiptUrl validation (optional)
        When(x => !string.IsNullOrEmpty(x.ReceiptUrl), () =>
        {
            RuleFor(x => x.ReceiptUrl)
                .MaximumLength(500).WithMessage("Receipt URL cannot exceed 500 characters")
                .Must(BeAValidUrl).WithMessage("Receipt URL must be a valid URL")
                .Must(BeASecureUrl).When(x => x.ReceiptUrl?.StartsWith("http://") == true)
                .WithMessage("Receipt URL should use HTTPS for security")
                .Must(BeAnAllowedFileType).WithMessage("Receipt URL must point to an allowed file type (jpg, jpeg, png, pdf)");
        });

        // Cross-property validation
        RuleFor(x => x)
            .Must(HaveReasonableAmountForDescription).WithMessage("Amount seems too high for the given description")
            .Must(HaveRecentDateForLargeAmounts).WithMessage("Large expenses must be from the current month");
    }

    private bool BeAReasonableDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return false;

        // Check for spam patterns
        var spamIndicators = new[] { "http://", "https://", "www.", ".com", ".net", "FREE", "WINNER", "URGENT" };
        if (spamIndicators.Any(indicator => description.Contains(indicator, StringComparison.OrdinalIgnoreCase)))
            return false;

        // Check for repeated characters
        if (description.Distinct().Count() < 3 && description.Length > 10)
            return false;

        return true;
    }

    private bool BeAReasonableAmount(decimal amount)
    {
        // Check for suspicious amounts (like 999999, 123456, etc.)
        var suspiciousAmounts = new[] { 999999m, 888888m, 123456m, 1000000m };
        if (suspiciousAmounts.Contains(amount))
            return false;

        // Check for round numbers that might be placeholders
        if (amount % 1000 == 0 && amount > 10000)
            return false;

        return true;
    }

    private bool BeAValidBusinessDate(DateTime date)
    {
        // Check if date is weekend (you can add holiday checks if needed)
        return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    private bool BeASecureUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    }

    private bool BeAnAllowedFileType(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".webp" };
        return allowedExtensions.Any(ext =>
            url.EndsWith(ext, StringComparison.OrdinalIgnoreCase) ||
            url.Contains(ext + "?", StringComparison.OrdinalIgnoreCase));
    }

    private bool HaveReasonableAmountForDescription(ExpenseCommand command)
    {
        // Define reasonable amount limits based on description patterns
        var highValueKeywords = new[] { "laptop", "equipment", "furniture", "software", "training" };
        var lowValueKeywords = new[] { "coffee", "lunch", "snack", "transport", "parking" };

        var description = command.Description.ToLowerInvariant();

        if (highValueKeywords.Any(keyword => description.Contains(keyword)))
        {
            // High-value items should have reasonable amounts
            return command.Amount <= 5000; // $5,000 limit for high-value items
        }

        if (lowValueKeywords.Any(keyword => description.Contains(keyword)))
        {
            // Low-value items should have smaller amounts
            return command.Amount <= 100; // $100 limit for low-value items
        }

        // Default limit for other expenses
        return command.Amount <= 1000; // $1,000 default limit
    }

    private bool HaveRecentDateForLargeAmounts(ExpenseCommand command)
    {
        // Large amounts should be from recent dates
        if (command.Amount > 1000)
        {
            return command.Date >= DateTime.UtcNow.AddMonths(-1);
        }

        return true;
    }
}