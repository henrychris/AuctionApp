using AuctionApp.Application.Contracts;
using AuctionApp.Domain.Entities;
using AuctionApp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace AuctionApp.Infrastructure.Services
{
    public class RoomService(DataContext context) : IRoomService
    {
        public async Task<BiddingRoom?> GetRoomAsync(string biddingRoomId)
        {
            return await context.BiddingRooms.FindAsync(biddingRoomId);
        }

        // get room and include auction
        public async Task<BiddingRoom?> GetRoomWithAuctionAsync(string biddingRoomId)
        {
            return await context.BiddingRooms.Include(r => r.Auction).FirstOrDefaultAsync(r => r.Id == biddingRoomId);
        }
    }
}