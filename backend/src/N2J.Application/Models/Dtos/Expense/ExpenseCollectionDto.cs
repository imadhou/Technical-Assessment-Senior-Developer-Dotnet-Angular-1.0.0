using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Models.Dtos.Expense
{
    public class ExpenseCollectionDto
    {
        public ICollection<ExpenseDto> Expenses { get; set; } = null!;
        public ICollection<LinkDto> Links { get; set; } = null!;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

    }
}
