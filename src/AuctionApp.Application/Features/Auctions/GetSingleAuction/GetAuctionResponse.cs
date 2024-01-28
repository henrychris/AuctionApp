namespace AuctionApp.Application.Features.Auction.GetSingleAuction;

public class GetAuctionResponse
{
	public required string AuctionId { get; set; }
	public required string Name { get; set; }
	public required decimal StartingPriceInNaira { get; set; }
	public required DateTime StartingTime { get; set; }
	public required DateTime ClosingTime { get; set; }
	public required decimal HighestBidAmountInNaira { get; set; }
	public required string AuctionStatus { get; set; }
}