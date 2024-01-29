namespace AuctionApp.Domain.ServiceErrors;

public static partial class Errors
{
    public static class BiddingRoom
    {
        public static Error Closed => Error.Unexpected(
            code: "BiddingRoom.Closed",
            description: "This bidding room is closed.");
    }
}