using System.Security.Claims;

using AuctionApp.Domain.Constants;
using AuctionApp.Domain.Entities.Hub;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AuctionApp.Infrastructure.Hubs;

[Authorize]
public class AuctionRoomHub(ILogger<AuctionRoomHub> logger) : Hub
{
    private string GetUserId()
    {
        return Context.User!.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    private string GetUserName()
    {
        return Context.User!.FindFirst(JwtClaims.FIRST_NAME)?.Value ?? string.Empty;
    }

    public async Task SendMessageToRoom(string roomId, string content)
    {
        var userId = GetUserId();
        logger.LogInformation("User {userId} sent message to room {roomId}.", userId, roomId);
        var message = new Message(GetUserName(), content);
        await Clients.Group(roomId).SendAsync("ReceiveMessage", message);
    }
}