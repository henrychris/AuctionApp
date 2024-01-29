using System.ComponentModel.DataAnnotations;

using AuctionApp.Domain.Constants;
using AuctionApp.Domain.Entities.Base;
using AuctionApp.Domain.Enums;

namespace AuctionApp.Domain.Entities;

public class Auction : BaseEntity
{
    public Auction()
    {
        // Initialize HighestBidAmount to StartingPrice
        HighestBidAmountInKobo = StartingPriceInKobo;
    }

    /// <summary>
    /// The name of the item up for auction.
    /// </summary>
    [MaxLength(DomainConstants.MAX_NAME_LENGTH)]
    public required string Name { get; set; }

    /// <summary>
    /// The starting price in kobo.
    /// <remarks>Please remember to divide by 100 when returning to frontend, and multiply by 100 when storing
    /// on the backend.</remarks>
    /// </summary>
    public required int StartingPriceInKobo { get; set; }

    /// <summary>
    /// The date and time the auction starts.
    /// </summary>
    public required DateTime StartingTime { get; set; }

    /// <summary>
    /// The date and time the auction ends.
    /// </summary>
    public required DateTime ClosingTime { get; set; }

    /// <summary>
    /// The highest bid placed in kobo.
    /// <remarks>Please remember to divide by 100 when returning to frontend, and multiply by 100 when storing
    /// on the backend.</remarks>
    /// </summary>
    public int HighestBidAmountInKobo { get; set; }

    /// <summary>
    /// The id of the user who placed the highest bid.
    ///
    /// <remarks>
    /// This is nullable because the auction may not have started yet, or there may not be any bids yet. Make sure this ID belongs to a valid user.
    /// </remarks>
    /// </summary>
    public string? HighestBidderId { get; set; }

    /// <summary>
    /// The current state of the auction.
    /// </summary>
    public AuctionStatus Status { get; set; } = AuctionStatus.NotStarted;

    public bool IsInProgress()
    {
        return Status == AuctionStatus.InProgress;
    }

    public void Start()
    {
        Status = AuctionStatus.InProgress;
    }
}