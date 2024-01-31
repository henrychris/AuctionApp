using AuctionApp.Application.Features.Rooms.EndRoomAuction;
using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Contracts
{
    public interface IRoomService
    {
        Task<BiddingRoom?> GetRoomAsync(string biddingRoomId);
        Task<BiddingRoom?> GetRoomWithAuctionAsync(string biddingRoomId);
        Task<BiddingRoom?> GetRoomWithAuctionAndBidsAsync(string biddingRoomId);
        Task CreateRoomAsync(BiddingRoom room);
        Task UpdateRoomAsync(BiddingRoom room);
        Task SendAuctionStartMessageToClientsAsync(string roomId, string productName,
                                                   decimal startingPriceInNaira);

        Task AnnounceNewHighestBidAsync(BiddingRoom room, Bid bid, string firstName);
        IQueryable<BiddingRoom> GetRoomsQuery();
        Task AnnounceEndOfAuction(EndAuctionDto endAuctionDto);
        Task KickAllUsersFromGroup(string roomId);
    }
}