using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Models.Commands
{
    public class ExpenseStatusUpdateCommand
    {
        public string NewStatus { get; set; } = string.Empty;
    }
}
