
using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Contracts
{
    public interface IRoomService
    {
        Task<BiddingRoom?> GetRoomAsync(string biddingRoomId);
        Task<BiddingRoom?> GetRoomWithAuctionAsync(string biddingRoomId);
    }
}