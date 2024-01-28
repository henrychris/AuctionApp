using AuctionApp.Application.Contracts;
using AuctionApp.Infrastructure.Services;

namespace AuctionApp.Host.Configuration;

public static class ServiceConfiguration
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IAuctionService, AuctionService>();
        services.AddScoped<IBidService, BidService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddSingleton(TimeProvider.System);
    }
}