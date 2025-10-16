using FluentValidation;
using N2J.Application.Interfaces;
using N2J.Application.Mappers;
using N2J.Application.Services;
using N2J.Application.Validators;

namespace N2J.WebApi.Extensions
{
    public static class AddApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(cfg => cfg.AddProfile<EntityMappingProfile>());

            // FluentValidation
            services.AddValidatorsFromAssemblyContaining(typeof(ExpenseQueryValidator));

            // Application services
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IUserAuthService, UserAuthService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IReceiptService, ReceiptService>();

            return services;
        }
    }
}
