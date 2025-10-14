using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using N2J.Application.Common.Mappings;
using N2J.Application.Interfaces;
using N2J.Application.Services;
using N2J.Domain.Interfaces;
using N2J.Infrastructure.Persistence.Repositories;


namespace N2J.WebApi.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(cfg => cfg.AddProfile<DtoEntityMappingProfile>());

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining(typeof(ExpenseQueryValidator));

        // Application services
        services.AddScoped<IExpenseService, ExpenseService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        return services;
    }

}
