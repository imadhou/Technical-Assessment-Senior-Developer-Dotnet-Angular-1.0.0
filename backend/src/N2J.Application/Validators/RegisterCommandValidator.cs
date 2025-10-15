using FluentValidation;
using N2J.Application.Models.Commands.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email address is required")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

            // Password validation
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]").WithMessage("Password must contain at least one special character");

            // FirstName validation
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MinimumLength(2).WithMessage("First name must be at least 2 characters long")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
                .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("First name can only contain letters, spaces, hyphens, apostrophes, and periods")
                .Must(NotContainNumbersOrSpecialChars).WithMessage("First name cannot contain numbers or special characters");

            // LastName validation
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters long")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
                .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("Last name can only contain letters, spaces, hyphens, apostrophes, and periods")
                .Must(NotContainNumbersOrSpecialChars).WithMessage("Last name cannot contain numbers or special characters");

            // CompanyId validation (optional)
            When(x => x.CompanyId.HasValue, () =>
            {
                RuleFor(x => x.CompanyId)
                    .NotEqual(Guid.Empty).WithMessage("Company ID must be a valid GUID");
            });
        }

        private bool BeAValidDomain(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var domain = email.Split('@')[1];

                // Check for valid domain format
                return domain.Contains('.') &&
                       domain.Length >= 4 && // a.co
                       !domain.StartsWith(".") &&
                       !domain.EndsWith(".");
            }
            catch
            {
                return false;
            }
        }

        private bool NotBeDisposableEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var disposableDomains = new[]
            {
            "tempmail.com", "guerrillamail.com", "mailinator.com", "10minutemail.com",
            "throwawaymail.com", "fakeinbox.com", "yopmail.com", "trashmail.com",
            "disposablemail.com", "temp-mail.org"
        };

            var domain = email.Split('@')[1].ToLowerInvariant();
            return !disposableDomains.Any(d => domain.Contains(d));
        }

        private bool BeInLowerCase(string email)
        {
            return email == email.ToLowerInvariant();
        }

        private bool NotContainPersonalInfo(RegisterCommand command, string password)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(command.FirstName))
                return true;

            var personalInfo = new[]
            {
            command.FirstName.ToLowerInvariant(),
            command.LastName.ToLowerInvariant(),
            command.Email.Split('@')[0].ToLowerInvariant()
        };

            return !personalInfo.Any(info =>
                !string.IsNullOrEmpty(info) &&
                password.ToLowerInvariant().Contains(info));
        }

        private bool NotBeCommonPassword(string password)
        {
            var commonPasswords = new[]
            {
            "password", "12345678", "qwertyui", "admin123", "welcome1",
            "password1", "123456789", "abc12345", "letmein1", "monkey12"
        };

            return !commonPasswords.Contains(password.ToLowerInvariant());
        }

        private bool NotContainSequentialChars(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return true;

            // Check for sequential characters (123, abc, etc.)
            for (int i = 0; i < password.Length - 2; i++)
            {
                var current = password[i];
                var next = password[i + 1];
                var nextNext = password[i + 2];

                // Check numeric sequences
                if (char.IsDigit(current) && char.IsDigit(next) && char.IsDigit(nextNext))
                {
                    if (current + 1 == next && next + 1 == nextNext)
                        return false;
                }

                // Check alphabetical sequences (case insensitive)
                if (char.IsLetter(current) && char.IsLetter(next) && char.IsLetter(nextNext))
                {
                    var currentLower = char.ToLower(current);
                    var nextLower = char.ToLower(next);
                    var nextNextLower = char.ToLower(nextNext);

                    if (currentLower + 1 == nextLower && nextLower + 1 == nextNextLower)
                        return false;
                }
            }

            return true;
        }

        private bool NotContainRepeatedChars(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return true;

            // Check for repeated characters (aaa, 111, etc.)
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (password[i] == password[i + 1] && password[i] == password[i + 2])
                    return false;
            }

            return true;
        }

        private bool NotContainNumbersOrSpecialChars(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;

            return !name.Any(char.IsDigit) &&
                   !name.Any(c => "!@#$%^&*()_+={}[]:;\"'<>?,./".Contains(c));
        }

        private bool BeAValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Check for reasonable name patterns
            if (name.Length < 2)
                return false;

            // Check for mixed case (should be properly capitalized)
            if (!char.IsUpper(name[0]))
                return false;

            // Check for consecutive special characters
            if (name.Contains("--") || name.Contains("''") || name.Contains(".."))
                return false;

            // Check for names that are too generic
            var genericNames = new[] { "test", "admin", "user", "fake", "dummy" };
            if (genericNames.Contains(name.ToLowerInvariant()))
                return false;

            return true;
        }

        private bool HaveDifferentFirstNameAndLastName(RegisterCommand command)
        {
            return !string.Equals(command.FirstName, command.LastName, StringComparison.OrdinalIgnoreCase);
        }

        private bool HavePasswordDifferentFromPersonalInfo(RegisterCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Password))
                return true;

            var personalInfo = new[]
            {
            command.FirstName,
            command.LastName,
            command.Email.Split('@')[0]
        };

            return !personalInfo.Any(info =>
                !string.IsNullOrEmpty(info) &&
                command.Password.ToLowerInvariant().Contains(info.ToLowerInvariant()));
        }

        private bool HaveReasonableNameLength(RegisterCommand command)
        {
            var totalNameLength = (command.FirstName?.Length ?? 0) + (command.LastName?.Length ?? 0);
            return totalNameLength >= 4 && totalNameLength <= 80;
        }
    }
}
