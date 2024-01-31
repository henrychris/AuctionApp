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
            options.UseSqlite(connectionString, o => o.MigrationsHistoryTable(
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
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

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
        await SeedUsers(userManager);
        var auction = await SeedAuctions(context);
        await SeedBiddingRooms(context, auction);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBiddingRooms(DataContext context, Auction auction)
    {
        var biddingRoom = new BiddingRoom
        {
            Id = "biddingRoom1", AuctionId = "auction1", Status = RoomStatus.Open, Auction = auction
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

        await context.Auctions.AddAsync(auction);
        Console.WriteLine("Auction seeding complete.");
        return auction;
    }

    private static async Task SeedUsers(UserManager<User> userManager)
    {
        var admin = CreateUser("Henry", "test@email.com", "User", "c0bdebd1-f275-4722-aa54-ca4524e4b998");
        var user = CreateUser("User", "test2@email.com", "User", "testUserId");
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