using AuctionApp.Infrastructure.Hubs;

namespace AuctionApp.Host.Configuration;

public static class SignalRConfiguration
{
    /// <summary>
    /// Configure SignalR hubs.
    /// </summary>
    /// <param name="app"></param>
    public static void AddSignalRHubs(this WebApplication app)
    {
        app.MapHub<AuctionRoomHub>("/auctionHub");
    }
}