using System.Text.Json;
using System.Text.Json.Serialization;

using AuctionApp.Infrastructure.Filters;
using AuctionApp.Infrastructure.Middleware;

using FluentValidation;

using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApp.Host.Configuration;

public static class StartupConfiguration
{
    public static void SetupControllers(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddRouting(options => options.LowercaseUrls = true);
        // validation is performed using FluentValidation and a specific response body is used.
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
    }

    public static void SetupFilters(this IServiceCollection services)
    {
        services.AddScoped<CustomValidationFilter>();
    }

    public static void SetupJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(jsonOptions =>
        {
            jsonOptions.JsonSerializerOptions.WriteIndented = false;
            jsonOptions.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            jsonOptions.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });
    }

    public static void AddFeatures(this IServiceCollection services)
    {
        var assemblyToScan = typeof(LoginRequest).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assemblyToScan));
        services.AddValidatorsFromAssembly(assemblyToScan);
    }
    
    public static void RegisterMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseMiddleware<ExceptionMiddleware>();
    }
}