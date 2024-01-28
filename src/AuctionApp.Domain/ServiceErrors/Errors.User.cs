namespace AuctionApp.Domain.ServiceErrors;

public static partial class Errors
{
    public static class User
    {
        public static Error NotFound => Error.NotFound(
            code: "User.NotFound",
            description: "User not found.");

        public static Error DuplicateEmail => Error.Validation(
            code: "User.DuplicateEmail",
            description: "This email is already in use.");

        public static Error IsLockedOut => Error.Unauthorized(
            code: "User.IsLockedOut",
            description: "User is locked out. Please contact admin.");

        public static Error IsNotAllowed => Error.Unauthorized(
            code: "User.IsNotAllowed",
            description: "User is not allowed to access the system. Please contact admin.");

        public static Error ReconfirmEmail => Error.Validation(
            code: "User.ReconfirmEmail",
            description: "Something isn't right. Please confirm your details.");
    }
}