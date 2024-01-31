using AuctionApp.Domain.Entities.Hub;

namespace AuctionApp.Application.Contracts.SignalRClients;

public interface IAuctionRoomClient
{
    Task SendMessageToRoom(string roomName, string content);
    Task JoinRoom(string connectionId, string roomId);
    Task LeaveRoom(string connectionId, string roomId);
    Task CloseGroup(string roomId);

    Task UserLeft(string username);
    Task UserJoined(string username);
    Task KickUser(GroupUser? user);
    Task ReceiveMessage(Message message);
    Task AuctionStarted(string message);
    Task BidPlaced(Message message, decimal amountInNaira);
    Task AuctionEnded(string winnerName, decimal priceInNaira);
}