using AuctionApp.Infrastructure.Hubs;

namespace AuctionApp.Host.Configuration;

public static class SignalRConfiguration
{
    public static void AddSignalRHubs(this WebApplication app)
    {
        app.MapHub<AuctionRoomHub>("/auctionHub");
    }
}