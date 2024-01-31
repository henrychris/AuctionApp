using AuctionApp.Domain.Entities;
using AuctionApp.Domain.ServiceErrors;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Users.GetUser;

public class GetSingleUserRequest : IRequest<ErrorOr<GetUserResponse>>
{
    public required string UserId { get; init; }
}

public class GetSingleUserRequestHandler(UserManager<User> userManager, ILogger<GetSingleUserRequestHandler> logger)
    : IRequestHandler<GetSingleUserRequest, ErrorOr<GetUserResponse>>
{
    public async Task<ErrorOr<GetUserResponse>> Handle(GetSingleUserRequest request,
                                                       CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching user... UserID: {userId}", request.UserId);

        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            return SharedErrors<User>.NotFound;
        }


        return new GetUserResponse { FirstName = user.FirstName };
    }
}