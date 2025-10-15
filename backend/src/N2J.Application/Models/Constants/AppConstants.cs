using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Models.Constants
{
    public class AppConstants
    {
        public const string EmployeeRole = "Employee";
        public const string ManagerRole = "Manager";
        public const string AdminRole = "Admin";

        public const string ExpenseStatusPending = "Pending";
        public const string ExpenseStatusApproved = "Approved";
        public const string ExpenseStatusRejected = "Rejected";

        public static readonly Guid DefaultCompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    }
}
