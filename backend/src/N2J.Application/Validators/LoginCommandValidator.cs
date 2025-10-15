using FluentValidation;
using N2J.Application.Models.Commands.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Validators
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
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
        }
    }
}
