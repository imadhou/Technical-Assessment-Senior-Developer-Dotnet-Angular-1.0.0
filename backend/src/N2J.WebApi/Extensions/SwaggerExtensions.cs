using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace N2J.WebApi.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenOptionsSetup>();
        services.AddSwaggerGen();
        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "openapi/{documentName}.json";
        });

        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                var url = $"/openapi/{description.GroupName}.json";
                var name = $"API {description.GroupName.ToUpperInvariant()}";
                options.SwaggerEndpoint(url, name);
            }

            options.RoutePrefix = "swagger";
            options.DocumentTitle = "Expense Management API Docs";
            options.DisplayOperationId();
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.EnablePersistAuthorization();
            options.EnableFilter();
        });

        return app;
    }
}

public class SwaggerGenOptionsSetup : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public SwaggerGenOptionsSetup(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"Expense Management API {description.GroupName}",
                Version = description.ApiVersion.ToString(),
                Description = "RESTful API for managing expenses, users, companies, and categories.",
                Contact = new OpenApiContact
                {
                    Name = "N2J Development Team",
                    Email = "dev@n2j.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });
        }

        // Include XML comments
        var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
        foreach (var xmlFile in xmlFiles)
            options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);

        /* TODO: Enable JWT Auth in Swagger UI
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        Array.Empty<string>()
                    }
                });
        */
    }
}