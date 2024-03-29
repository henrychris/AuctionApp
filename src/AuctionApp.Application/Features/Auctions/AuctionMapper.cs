using AuctionApp.Application.Features.Auctions.CreateAuction;
using AuctionApp.Application.Features.Auctions.GetSingleAuction;
using AuctionApp.Common;

namespace AuctionApp.Application.Features.Auctions
{
    public static class AuctionMapper
    {
        internal static Domain.Entities.Auction CreateAuction(CreateAuctionRequest request)
        {
            return new Domain.Entities.Auction
            {
                Name = request.Name,
                StartingPriceInKobo = CurrencyConverter.ConvertNairaToKobo(request.StartingPriceInNaira),
                StartingTime = request.StartingTime,
                ClosingTime = request.ClosingTime,
                HighestBidAmountInKobo = CurrencyConverter.ConvertNairaToKobo(request.StartingPriceInNaira)
            };
        }


        internal static CreateAuctionResponse ToCreateAuctionResponse(Domain.Entities.Auction auction)
        {
            return new CreateAuctionResponse
            {
                AuctionId = auction.Id
            };
        }


        internal static GetAuctionResponse ToGetAuctionResponse(Domain.Entities.Auction auction)
        {
            return new GetAuctionResponse
            {
                AuctionId = auction.Id,
                Name = auction.Name,
                StartingPriceInNaira = CurrencyConverter.ConvertKoboToNaira(auction.StartingPriceInKobo),
                StartingTime = auction.StartingTime,
                ClosingTime = auction.ClosingTime,
                HighestBidAmountInNaira = CurrencyConverter.ConvertKoboToNaira(auction.HighestBidAmountInKobo),
                AuctionStatus = auction.Status.ToString()
            };
        }

    }
}