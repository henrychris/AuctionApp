namespace AuctionApp.Domain.ServiceErrors
{
    public static partial class Errors
    {
        public static class Bid
        {
            public static Error UserAlreadyHighestBidder => Error.Validation(
                code: "Bid.UserAlreadyHighestBidder",
                description: "You are already the highest bidder.");

            public static Error AmountTooLow => Error.Validation(
                code: "Bid.AmountTooLow",
                description: "Bid amount is too low.");

            public static Error AmountNotHigherThanCurrentHighestBid => Error.Validation(
                code: "Bid.AmountNotHigherThanCurrentHighestBid",
                description: "Bid amount is not higher than current highest bid.");

        }
    }
}