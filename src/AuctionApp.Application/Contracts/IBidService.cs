using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Contracts
{
    public interface IBidService
    {
        Task CreateBidAsync(Bid bid);
        bool IsBidAmountHigherThanCurrentBid(BiddingRoom room, decimal amountInNaira);
        bool IsBidAmountTooLow(decimal amountInNaira);
        bool IsUserAlreadyHighestBidder(BiddingRoom room, string? userId);

    }
}