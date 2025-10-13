using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace N2J.WebApi.Startup;

public static class JsonConfiguration
{
    public static IMvcBuilder AddCustomJsonOptions(this IMvcBuilder builder)
    {
        return builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });
    }
}