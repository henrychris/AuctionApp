using AuctionApp.Application.Extensions;
using AuctionApp.Application.Features.Auth.Register;
using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Features.Auth;

public static class AuthMapper
{
    public static User MapToUser(RegisterRequest request)
    {
        return new User
        {
            FirstName = request.FirstName.FirstCharToUpper(),
            LastName = request.LastName.FirstCharToUpper(),
            Email = request.EmailAddress,
            UserName = request.EmailAddress,
            Role = request.Role
        };
    }
}