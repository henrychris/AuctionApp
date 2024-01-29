namespace AuctionApp.Domain.ServiceErrors;

public static partial class Errors
{
    public static class BiddingRoom
    {
        public static Error Closed => Error.Unexpected(
            code: "BiddingRoom.Closed",
            description: "This bidding room is closed.");

        public static Error NotOpen => Error.Unexpected(
            code: "BiddingRoom.NotOpen",
            description: "This bidding room is not open.");
    }
}