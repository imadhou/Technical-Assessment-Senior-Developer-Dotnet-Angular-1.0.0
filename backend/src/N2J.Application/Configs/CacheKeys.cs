using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Configs
{
    public static class CacheKeys
    {
        public static string ExpensesByCompany(Guid companyId, int page, int pageSize, string sortBy, bool isDescending)
            => $"expenses:company:{companyId}:page:{page}:size:{pageSize}:sort:{sortBy}:desc:{isDescending}";

        public static string ExpensesByCompanyAndStatus(Guid companyId, string status, int page, int pageSize, string sortBy, bool isDescending)
            => $"expenses:company:{companyId}:status:{status}:page:{page}:size:{pageSize}:sort:{sortBy}:desc:{isDescending}";

        public static string ExpensesByCompanyAndCategory(Guid companyId, Guid expenseCategoryId, int page, int pageSize, string sortBy, bool isDescending)
            => $"expenses:company:{companyId}:category:{expenseCategoryId}:page:{page}:size:{pageSize}:sort:{sortBy}:desc:{isDescending}";

        public static string ExpenseById(Guid id) => $"expense:{id}";
    }
}
