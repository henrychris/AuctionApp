using AuctionApp.Application.Features.Bids.MakeBid;
using AuctionApp.Common;
using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Features.Bids
{
    public static class BidMapper
    {
        internal static Bid CreateBid(MakeBidRequest request, string? userId)
        {
            return new Bid
            {
                AmountInKobo = CurrencyConverter.ConvertNairaToKobo(request.BidAmountInNaira),
                BiddingRoomId = request.RoomId,
                UserId = userId!
            };
        }

        internal static MakeBidResponse CreateBidResponse(Bid bid)
        {
            return new MakeBidResponse { BidId = bid.Id };
        }
    }
}