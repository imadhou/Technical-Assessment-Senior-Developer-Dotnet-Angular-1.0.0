using Microsoft.AspNetCore.Mvc;

namespace N2J.WebApi.Startup;

public static class ModelValidationConfiguration
{
    public static IMvcBuilder AddCustomModelValidation(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var result = new BadRequestObjectResult(new
                {
                    Message = "Validation failed",
                    Errors = errors,
                    StatusCode = 400,
                    TraceId = context.HttpContext.TraceIdentifier
                });

                return result;
            };
        });

        return builder;
    }
}