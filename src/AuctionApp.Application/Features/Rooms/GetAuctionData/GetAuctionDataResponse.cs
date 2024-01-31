namespace AuctionApp.Application.Features.Rooms.GetAuctionData;

public class GetAuctionDataResponse
{
    public required decimal HighestBidAmountInNaira { get; init; }
    public required string AuctionStatus { get; init; }
    public required string NameOfHighestBidder { get; init; }
}