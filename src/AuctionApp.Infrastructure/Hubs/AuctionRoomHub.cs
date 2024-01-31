using System.Collections.Concurrent;
using System.Security.Claims;

using AuctionApp.Application.Contracts.SignalRClients;
using AuctionApp.Domain.Constants;
using AuctionApp.Domain.Entities.Hub;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AuctionApp.Infrastructure.Hubs;



[Authorize]
public class AuctionRoomHub(ILogger<AuctionRoomHub> logger) : Hub<IAuctionRoomClient>
{
    private static ConcurrentDictionary<string, List<GroupUser>> groupUsers = new();

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
        await Clients.Group(roomId).ReceiveMessage(message);
    }

    public async Task JoinRoom(string connectionId, string roomId)
    {
        await Groups.AddToGroupAsync(connectionId, roomId);
        if (!groupUsers.TryGetValue(roomId, out List<GroupUser>? value))
        {
            value = ( []);
            groupUsers[roomId] = value;
        }

        value.Add(new GroupUser(GetUserName(), connectionId));
        await Clients.Group(roomId).UserJoined(GetUserName());
    }

    public async Task LeaveRoom(string connectionId, string roomId)
    {
        if (groupUsers.TryGetValue(roomId, out List<GroupUser>? value))
        {
            value.Remove(new GroupUser(GetUserName(), connectionId));
            await Groups.RemoveFromGroupAsync(connectionId, roomId);
            await Clients.Group(roomId).UserLeft(GetUserName());
        }
    }
    
    public async Task CloseGroup(string roomId)
    {
        // Get the list of users in the group
        if (groupUsers.TryGetValue(roomId, out var users))
        {
            // Kick each user from the group
            foreach (var user in users)
            {
                await Clients.Group(roomId).KickUser(user);
                groupUsers[roomId].Remove(user);
            }

            // Remove the group from the dictionary
            groupUsers.Remove(roomId, out var value);
        }
    }
}