using System.Diagnostics;

using AuctionApp.Domain.Constants;
using AuctionApp.Domain.Settings;
using AuctionApp.Infrastructure.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;

namespace AuctionApp.Host.Configuration;

public static class DatabaseConfiguration
{
    private const string IN_MEMORY_PROVIDER_NAME = "Microsoft.EntityFrameworkCore.InMemory";

    private static bool IsInMemoryDatabase(DbContext context)
    {
        return context.Database.ProviderName == IN_MEMORY_PROVIDER_NAME;
    }


    public static void SetupDatabase<T>(this IServiceCollection services) where T : DbContext
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Console.WriteLine($"Current Environment: {env}");
        string connectionString;

        if (env == Environments.Development)
        {
            var dbSettings = services.BuildServiceProvider().GetService<IOptionsSnapshot<DatabaseSettings>>()?.Value;
            connectionString = dbSettings!.ConnectionString!;
        }
        // else if (env == prod), use postgres production string
        else
        {
            // when running integration tests
            return;
        }

        services.AddDbContext<T>(options =>
        {
            options.UseNpgsql(connectionString, o => o.MigrationsHistoryTable(
                tableName: HistoryRepository.DefaultTableName, typeof(T).Name));
        });
    }


    public static async Task SeedDatabase(this WebApplication app)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        Console.WriteLine("Database seeding starting.");
        await SeedDatabaseInternal(app);

        stopwatch.Stop();
        var elapsedTime = stopwatch.Elapsed;
        Console.WriteLine($"Database seeding completed in {elapsedTime.TotalMilliseconds}ms.");
    }

    private static async Task SeedDatabaseInternal(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (IsInMemoryDatabase(context))
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        await SeedRoles(roleManager);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Roles.AllRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($"Created role: {role}.");
            }
        }

        Console.WriteLine("Role seeding complete.");
    }
}