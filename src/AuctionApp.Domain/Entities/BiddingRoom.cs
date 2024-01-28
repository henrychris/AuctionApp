using AuctionApp.Domain.Entities.Base;
using AuctionApp.Domain.Enums;

namespace AuctionApp.Domain.Entities;

public class BiddingRoom : BaseEntity
{
    public RoomStatus Status { get; set; } = RoomStatus.Closed;

    public required string AuctionId { get; set; }
    public Auction Auction { get; set; } = null!;

    public ICollection<Bid> Bids { get; set; } = [];
}