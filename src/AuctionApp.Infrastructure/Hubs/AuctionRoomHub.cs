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

    /// <summary>
    /// Gets the user's ID from the current context.
    /// </summary>
    /// <returns>The user's ID if it exists, otherwise an empty string.</returns>
    private string GetUserId()
    {
        return Context.User!.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Gets the user's name from the current context.
    /// </summary>
    /// <returns>The user's name if it exists, otherwise an empty string.</returns>
    private string GetUserName()
    {
        return Context.User!.FindFirst(JwtClaims.FIRST_NAME)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Sends a message to a specific room.
    /// </summary>
    /// <param name="roomId">The ID of the room to send the message to.</param>
    /// <param name="content">The content of the message.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SendMessageToRoom(string roomId, string content)
    {
        var userId = GetUserId();
        logger.LogInformation("User {userId} sent message to room {roomId}.", userId, roomId);
        var message = new Message(GetUserName(), content);
        await Clients.Group(roomId).ReceiveMessage(message);
    }

    /// <summary>
    /// Adds a user to a specified room.
    /// Note that rooms are Groups in SignalR. A group is a collection of connections.
    /// </summary>
    /// <param name="connectionId">The connection ID of the user.</param>
    /// <param name="roomId">The ID of the room to join.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    [Authorize(Roles = Roles.USER)]
    public async Task JoinRoom(string connectionId, string roomId)
    {
        await Groups.AddToGroupAsync(connectionId, roomId);
        if (!groupUsers.TryGetValue(roomId, out List<GroupUser>? value))
        {
            value = ([]);
            groupUsers[roomId] = value;
        }

        value.Add(new GroupUser(GetUserName(), connectionId));
        await Clients.Group(roomId).UserJoined(GetUserName());
    }

    /// <summary>
    /// Removes a user from a specific room.
    /// </summary>
    /// <param name="connectionId">The connection ID of the user.</param>
    /// <param name="roomId">The ID of the room to leave.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    [Authorize(Roles = Roles.USER)]
    public async Task LeaveRoom(string connectionId, string roomId)
    {
        if (groupUsers.TryGetValue(roomId, out List<GroupUser>? value))
        {
            value.Remove(new GroupUser(GetUserName(), connectionId));
            await Groups.RemoveFromGroupAsync(connectionId, roomId);
            await Clients.Group(roomId).UserLeft(GetUserName());
        }
    }

    /// <summary>
    /// Closes a specified group and kicks all users, if any.
    /// </summary>
    /// <param name="roomId">The ID of the group to close.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    [Authorize(Roles = Roles.ADMIN)]
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