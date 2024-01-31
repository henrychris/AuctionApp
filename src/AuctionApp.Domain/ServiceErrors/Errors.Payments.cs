namespace AuctionApp.Domain.ServiceErrors;

public static partial class Errors
{
    public static class Payments
    {
        public static Error AlreadyCompleted => Error.Conflict(
            code: "Payments.AlreadyCompleted",
            description: "This payment is already completed.");
    }
}