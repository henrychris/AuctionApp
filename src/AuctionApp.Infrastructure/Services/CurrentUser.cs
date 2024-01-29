using AuctionApp.Application.Contracts;
using AuctionApp.Domain.Constants;

namespace AuctionApp.Infrastructure.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string UserId =>
        httpContextAccessor.HttpContext?.User.FindFirst(JwtClaims.USER_ID)?.Value ??
        throw new InvalidOperationException("User not authorised.");

    public string Email =>
        httpContextAccessor.HttpContext?.User.FindFirst(JwtClaims.EMAIL)?.Value ??
        throw new InvalidOperationException("User not authorised.");

    public string Role =>
        httpContextAccessor.HttpContext?.User.FindFirst(JwtClaims.ROLE)?.Value ??
        throw new InvalidOperationException("User not authorised.");

    public string FirstName =>
        httpContextAccessor.HttpContext?.User.FindFirst(JwtClaims.FIRST_NAME)?.Value ??
        throw new InvalidOperationException("User not authorised.");
}