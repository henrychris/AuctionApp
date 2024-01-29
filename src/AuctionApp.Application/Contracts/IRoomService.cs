using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Contracts
{
    public interface IRoomService
    {
        Task<BiddingRoom?> GetRoomAsync(string biddingRoomId);
        Task<BiddingRoom?> GetRoomWithAuctionAsync(string biddingRoomId);
        Task CreateRoomAsync(BiddingRoom room);
        Task UpdateRoomAsync(BiddingRoom room);
        Task AddUserToRoom(string roomId, string userName, string connectionId);
        Task RemoveUserFromRoom(string userId, string roomId, string connectionId);
        Task SendAuctionStartMessageToClientsAsync(string roomId, string productName,
                                                   decimal startingPriceInNaira);

        Task AnnounceNewHighestBidAsync(BiddingRoom room, Bid bid, string firstName);
    }
}