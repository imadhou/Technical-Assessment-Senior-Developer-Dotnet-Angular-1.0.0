using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Configs
{
    public static class CacheKeys
    {
        // By company only
        public static string ExpensesByCompany(Guid companyId, int page, int pageSize, string sortBy, bool isDescending)
            => $"expenses:company:{companyId}:page:{page}:size:{pageSize}:sort:{sortBy}:desc:{isDescending}";


        // By company and category
        public static string ExpensesByCompanyAndCategory(Guid companyId, Guid categoryId, int page, int pageSize, string sortBy, bool isDescending)
            => $"expenses:company:{companyId}:category:{categoryId}:page:{page}:size:{pageSize}:sort:{sortBy}:desc:{isDescending}";

        // By company and status
        public static string ExpensesByCompanyAndStatus(Guid companyId, string status, int page, int pageSize, string sortBy, bool isDescending)
            => $"expenses:company:{companyId}:status:{status}:page:{page}:size:{pageSize}:sort:{sortBy}:desc:{isDescending}";

        // By company, category, and status
        public static string ExpensesByCompanyCategoryAndStatus(Guid companyId, Guid categoryId, string status, int page, int pageSize, string sortBy, bool isDescending)
            => $"expenses:company:{companyId}:category:{categoryId}:status:{status}:page:{page}:size:{pageSize}:sort:{sortBy}:desc:{isDescending}";

        public static string ExpensesByCompanyAndUser(Guid companyId, Guid userId, int page, int pageSize, string sortBy, bool isDescending)
        => $"expenses:company:{companyId}:user:{userId}:page:{page}:size:{pageSize}:sort:{sortBy}:desc:{isDescending}";

        public static string ExpensesByCompanyCategoryAndUser(Guid companyId, Guid categoryId, Guid userId, int page, int pageSize, string sortBy, bool isDescending)
            => $"expenses:company:{companyId}:category:{categoryId}:user:{userId}:page:{page}:size:{pageSize}:sort:{sortBy}:desc:{isDescending}";

        public static string ExpensesByCompanyStatusAndUser(Guid companyId, string status, Guid userId, int page, int pageSize, string sortBy, bool isDescending)
            => $"expenses:company:{companyId}:status:{status}:user:{userId}:page:{page}:size:{pageSize}:sort:{sortBy}:desc:{isDescending}";

        public static string ExpensesByCompanyCategoryStatusAndUser(Guid companyId, Guid categoryId, string status, Guid userId, int page, int pageSize, string sortBy, bool isDescending)
            => $"expenses:company:{companyId}:category:{categoryId}:status:{status}:user:{userId}:page:{page}:size:{pageSize}:sort:{sortBy}:desc:{isDescending}";

        // By expense id
        public static string ExpenseById(Guid id) => $"expense:{id}";
    }
}
