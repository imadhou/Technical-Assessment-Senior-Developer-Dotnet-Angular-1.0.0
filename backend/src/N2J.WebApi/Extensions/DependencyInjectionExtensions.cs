using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using N2J.Application.Common.Mappings;
using N2J.Application.Interfaces;
using N2J.Application.Models.Validators;
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

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Controllers and JSON
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
                options.JsonSerializerOptions.WriteIndented = true;
            })

        .ConfigureApiBehaviorOptions(options =>
         {
             // This enables FluentValidation to work with ModelState
             options.InvalidModelStateResponseFactory = context =>
             {
                 var errors = context.ModelState
                     .Where(e => e.Value.Errors.Count > 0)
                     .ToDictionary(
                         kvp => kvp.Key,
                         kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                     );

                 var result = new BadRequestObjectResult(new
                 {
                     Message = "Validation failed",
                     Errors = errors,
                     StatusCode = 400
                 });

                 return result;
             };
         });

        // API Versioning
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader(); // e.g. /api/v1/...
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV"; // v1, v2, etc.
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
}
