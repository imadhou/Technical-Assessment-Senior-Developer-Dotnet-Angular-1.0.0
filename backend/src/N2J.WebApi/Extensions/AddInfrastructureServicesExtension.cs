using N2J.Domain.Interfaces;
using N2J.Infrastructure.Persistence.Audit;
using N2J.Infrastructure.Persistence.Repositories;

namespace N2J.WebApi.Extensions
{
    public static class AddInfrastructureServicesExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IChangeLogRepository, ChangeLogRepository>();
            return services;
        }
    }
}
