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
        }
    }
}