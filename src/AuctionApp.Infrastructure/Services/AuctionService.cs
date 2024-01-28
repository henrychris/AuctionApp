using AuctionApp.Application.Contracts;
using AuctionApp.Domain.Entities;
using AuctionApp.Infrastructure.Data;

namespace AuctionApp.Infrastructure.Services;

public class AuctionService(DataContext context) : IAuctionService
{
    public async Task CreateAuctionAsync(Auction auction)
    {
        await context.Auctions.AddAsync(auction);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAuctionAsync(Auction auction)
    {
        context.Auctions.Remove(auction);
        await context.SaveChangesAsync();
    }

    public async Task<Auction?> GetAuctionByIdAsync(string auctionId)
    {
        return await context.Auctions.FindAsync(auctionId);
    }

    public IQueryable<Auction> GetAuctionsQuery()
    {
        return context.Auctions.AsQueryable();
    }

    public async Task UpdateAuctionAsync(Auction auction)
    {
        context.Auctions.Update(auction);
        await context.SaveChangesAsync();
    }

}