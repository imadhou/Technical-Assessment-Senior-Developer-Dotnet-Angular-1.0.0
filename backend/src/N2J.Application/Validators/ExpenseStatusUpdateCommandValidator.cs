using FluentValidation;
using N2J.Application.Models.Commands;
using N2J.Application.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Validators
{
    public class ExpenseStatusUpdateCommandValidator: AbstractValidator<ExpenseStatusUpdateCommand>
    {
        private static readonly List<string> AllowedStatuses = new List<string> { AppConstants.ExpenseStatusPending, AppConstants.ExpenseStatusApproved, AppConstants.ExpenseStatusRejected };
        public ExpenseStatusUpdateCommandValidator()
        {
            RuleFor(x => x.NewStatus)
                .NotEmpty().WithMessage("New status is required.")
                .Must(status => AllowedStatuses.Contains(status))
                .WithMessage($"Status must be one of the following: {string.Join(", ", AllowedStatuses)}");
        }
    }
}
