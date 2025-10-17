using N2J.Application.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Models.Commands
{

    public class ExpenseCommand
    {
        /// <summary>
        /// General description of the expense.
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Amount of the expense.
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Currency of the expense (e.g., USD, EUR).
        /// </summary>
        public string Currency { get; set; } = "EUR";
        /// <summary>
        /// Expense category identifier.
        /// </summary>
        public Guid ExpenseCategoryId { get; set; }
        /// <summary>
        /// Expense date in UTC.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Expense receipt URL.
        /// </summary>
        public string? ReceiptUrl { get; set; }
        /// <summary>
        /// User identifier who incurred the expense.
        /// </summary>
        public Guid UserId { get; set; }
    }
}
