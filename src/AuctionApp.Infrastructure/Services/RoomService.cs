using AuctionApp.Application.Contracts;
using AuctionApp.Common;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.Entities.Hub;
using AuctionApp.Infrastructure.Data;
using AuctionApp.Infrastructure.Hubs;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AuctionApp.Infrastructure.Services
{
    public class RoomService(DataContext context, IHubContext<AuctionRoomHub> hubContext) : IRoomService
    {
        public async Task<BiddingRoom?> GetRoomAsync(string biddingRoomId)
        {
            return await context.BiddingRooms.FindAsync(biddingRoomId);
        }

        public async Task<BiddingRoom?> GetRoomWithAuctionAsync(string biddingRoomId)
        {
            return await context.BiddingRooms.Include(r => r.Auction).FirstOrDefaultAsync(r => r.Id == biddingRoomId);
        }

        public async Task CreateRoomAsync(BiddingRoom room)
        {
            await context.BiddingRooms.AddAsync(room);
            await context.SaveChangesAsync();
        }

        public async Task UpdateRoomAsync(BiddingRoom room)
        {
            context.BiddingRooms.Update(room);
            await context.SaveChangesAsync();
        }

        public async Task SendAuctionStartMessageToClientsAsync(string roomId, string productName,
                                                                decimal startingPriceInNaira)
        {
            Console.WriteLine($"Sending auction start message to clients in room {roomId}");
            await hubContext.Clients.Group(roomId).SendAsync("AuctionStarted",
                $"Auction started for product {productName}. Starting Price: {startingPriceInNaira}NGN");
        }

        public async Task AnnounceNewHighestBidAsync(BiddingRoom room, Bid bid, string firstName)
        {
            var amountInNaira = CurrencyConverter.ConvertKoboToNaira(bid.AmountInKobo);
            var message = new Message(firstName, $"New highest bid for {room.Auction.Name} is {amountInNaira}NGN");

            Console.WriteLine($"Announcing new highest bid for room {room.Id}");
            await hubContext.Clients.Group(room.Id).SendAsync("BidPlaced", message, amountInNaira);
        }

        public IQueryable<BiddingRoom> GetRoomsQuery()
        {
            return context.BiddingRooms.AsQueryable();
        }
    }
}