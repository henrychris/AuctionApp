using AuctionApp.Domain.Entities.Hub;

namespace AuctionApp.Application.Contracts.SignalRClients;

public interface IAuctionRoomClient
{
    Task SendMessageToRoom(string roomName, string content);
    
    Task OnDisconnectedAsync(Exception? exception);
}