using System.Diagnostics;

using AuctionApp.Domain.Constants;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.Enums;
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

    /// <summary>
    /// Sets up the database context for the application.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
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
            // when running integration tests don't add any settings.
            return;
        }

        services.AddDbContext<T>(options =>
        {
            options.UseNpgsql(connectionString, o => o.MigrationsHistoryTable(
                tableName: HistoryRepository.DefaultTableName, typeof(T).Name));
        });
    }

    /// <summary>
    /// Seed the database with data.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
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
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        if (IsInMemoryDatabase(context))
        {
            // if the database is in-memory, then this is an integration test,
            // so we can delete and recreate the database on startup.
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            // this is a test application, so we can delete and recreate the database on startup.
            // a real application could apply migrations here, or do nothing.
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        await SeedRoles(roleManager);
        await SeedUsers(userManager);
        var auction = await SeedAuctions(context);
        await SeedBiddingRooms(context, auction);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBiddingRooms(DataContext context, Auction auction)
    {
        if (await context.BiddingRooms.AnyAsync())
        {
            return;
        }

        var biddingRoom = new BiddingRoom
        {
            Id = "biddingRoom1",
            AuctionId = "auction1",
            Status = RoomStatus.Open,
            Auction = auction
        };
        await context.BiddingRooms.AddAsync(biddingRoom);
        Console.WriteLine("Bidding room seeding complete.");
    }

    private static async Task<Auction> SeedAuctions(DataContext context)
    {
        var auction = new Auction
        {
            Id = "auction1",
            Name = "Auction 1",
            StartingPriceInKobo = 1000000,
            StartingTime = DateTime.UtcNow,
            ClosingTime = DateTime.UtcNow.AddDays(1),
            Status = AuctionStatus.InProgress
        };

        if (await context.Auctions.AnyAsync())
        {
            return auction;
        }

        await context.Auctions.AddAsync(auction);
        Console.WriteLine("Auction seeding complete.");
        return auction;
    }

    private static async Task SeedUsers(UserManager<User> userManager)
    {
        if (await userManager.Users.AnyAsync())
        {
            return;
        }

        var admin = CreateUser("Henry", "admin@email.com", "Admin", "c0bdebd1-f275-4722-aa54-ca4524e4b998");
        var user = CreateUser("User", "user@email.com", "User", "testUserId");
        await AddUser(userManager, admin, "testPassword123@");
        await AddUser(userManager, user, "testPassword123@");
        Console.WriteLine("User seeding complete.");
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

    private static User CreateUser(string userName, string email, string role, string userId)
    {
        return new User
        {
            Id = userId,
            FirstName = userName,
            LastName = "Ihenacho",
            Email = email,
            NormalizedEmail = email.ToUpper(),
            UserName = userName,
            NormalizedUserName = userName.ToUpper(),
            PhoneNumber = $"+1{userName.Length}1".PadLeft(12, '1'),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D"),
            Role = role
        };
    }

    private static async Task AddUser(UserManager<User> userManager, User user, string password)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (await userManager.FindByEmailAsync(user.Email!) is null)
        {
            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(user, password);
            user.PasswordHash = hashedPassword;

            await userManager.CreateAsync(user);
            await userManager.AddToRoleAsync(user, user.Role);
        }
    }
}