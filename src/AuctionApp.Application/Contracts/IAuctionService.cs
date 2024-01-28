using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Contracts;

public interface IAuctionService
{
    Task<Auction?> GetAuctionByIdAsync(string auctionId);

	Task CreateAuctionAsync(Auction auction);
	Task UpdateAuctionAsync(Auction auction);
	Task DeleteAuctionAsync(Auction auction);

	IQueryable<Auction> GetAuctionsQuery();
}