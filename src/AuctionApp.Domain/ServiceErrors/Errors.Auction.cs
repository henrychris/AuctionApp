namespace AuctionApp.Domain.ServiceErrors
{
    public static partial class Errors
    {
        public static class Auction
        {
            public static Error CannotDeleteInProgressAuction => Error.Conflict(
                code: "Auction.CannotDeleteInProgressAuction",
                description: "Cannot delete an auction that is in progress.");

            public static Error CannotUpdateAuctionInProgress => Error.Conflict(
                code: "Auction.CannotUpdateAuctionInProgress",
                description: "Cannot update an auction that is in progress.");

            public static Error AlreadyStarted => Error.Conflict(
                code: "Auction.AlreadyStarted",
                description: "Auction is already started.");

            public static Error NoBidsYet => Error.Failure(
                code: "Auction.NoBidsYet",
                description: "No bids have been placed on this auction");

            public static Error NotStartedYet => Error.Failure(
                code: "Auction.NotStartedYet",
                description: "This auction hasn't started yet.");
        }
    }
}