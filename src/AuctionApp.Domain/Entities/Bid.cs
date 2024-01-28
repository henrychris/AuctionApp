using AuctionApp.Domain.Entities.Base;

namespace AuctionApp.Domain.Entities;

public class Bid : BaseEntity
{
    /// <summary>
    /// The bid amount in kobo.
    /// <remarks>Please remember to divide by 100 when returning to frontend, and multiply by 100 when storing
    /// on the backend.</remarks>
    /// </summary>
    public required int AmountInKobo { get; set; }

    public required string UserId { get; set; }
    public User User { get; set; } = null!;

    public required string BiddingRoomId { get; set; }
    public BiddingRoom BiddingRoom { get; set; } = null!;
}