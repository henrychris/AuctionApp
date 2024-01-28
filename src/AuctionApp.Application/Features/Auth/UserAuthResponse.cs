namespace AuctionApp.Application.Features.Auth;

public class UserAuthResponse
{
    public required string Id { get; init; }
    public required string Role { get; init; }
    public required string AccessToken { get; init; }
}