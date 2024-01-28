using AuctionApp.Application.Contracts;
using AuctionApp.Common;
using AuctionApp.Domain.Entities;
using AuctionApp.Infrastructure.Data;

namespace AuctionApp.Infrastructure.Services
{
    public class BidService(DataContext context) : IBidService
    {
        private const int MINIMUM_BID_AMOUNT_IN_KOBO = 100_00;

        public async Task CreateBidAsync(Bid bid)
        {
            await context.Bids.AddAsync(bid);
            await context.SaveChangesAsync();
        }

        public bool IsBidAmountHigherThanCurrentBid(BiddingRoom room, decimal amountInNaira)
        {
            var amountInKobo = CurrencyConverter.ConvertNairaToKobo(amountInNaira);
            return amountInKobo > room.Auction.HighestBidAmountInKobo;
        }

        public bool IsBidAmountTooLow(decimal amountInNaira)
        {
            var amountInKobo = CurrencyConverter.ConvertNairaToKobo(amountInNaira);
            return amountInKobo < MINIMUM_BID_AMOUNT_IN_KOBO;
        }

        public bool IsUserAlreadyHighestBidder(BiddingRoom room, string? userId)
        {
            return room.Auction.HighestBidderId == userId;
        }

    }
}