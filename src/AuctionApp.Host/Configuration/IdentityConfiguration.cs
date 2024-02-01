using AuctionApp.Domain.Entities;
using AuctionApp.Infrastructure.Data;

using Microsoft.AspNetCore.Identity;

namespace AuctionApp.Host.Configuration;

public static class IdentityConfiguration
{
    public static void SetupMsIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 3;

                    // Lockout settings.
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.AllowedForNewUsers = true;

                    options.User.RequireUniqueEmail = true;
                    // options.SignIn.RequireConfirmedEmail = true;
                }).AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

        // generated tokens will only last 2 hours.
        services.Configure<DataProtectionTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromHours(2));
    }
}