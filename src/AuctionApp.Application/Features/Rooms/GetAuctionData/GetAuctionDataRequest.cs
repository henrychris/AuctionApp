using AuctionApp.Application.Contracts;
using AuctionApp.Common;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.ServiceErrors;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Rooms.GetAuctionData;

public class GetAuctionDataRequest : IRequest<ErrorOr<GetAuctionDataResponse>>
{
    public required string RoomId { get; set; }
}

public class GetAuctionDataRequestHandler(
    IRoomService roomService,
    UserManager<User> userManager,
    ILogger<GetAuctionDataRequestHandler> logger)
    : IRequestHandler<GetAuctionDataRequest, ErrorOr<GetAuctionDataResponse>>
{
    public async Task<ErrorOr<GetAuctionDataResponse>> Handle(GetAuctionDataRequest request,
                                                              CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching data for auction in room {id}.", request.RoomId);

        var roomWithAuction = await roomService.GetRoomWithAuctionAsync(request.RoomId);
        if (roomWithAuction is null)
        {
            logger.LogError("Room with id {id} does not exist.", request.RoomId);
            return SharedErrors<BiddingRoom>.NotFound;
        }

        var highestBidderId = roomWithAuction.Auction.HighestBidderId;
        if (highestBidderId is null)
        {
            logger.LogInformation("There are no bids on auction {auctionId}.", roomWithAuction.AuctionId);
            // return Errors.Auction.NoBidsYet;
        }

        string userFirstName = "N/A";
        if (highestBidderId is not null)
        {
            var user = await userManager.FindByIdAsync(highestBidderId);
            if (user is null)
            {
                logger.LogCritical("The supposed highest bidder does not exist. UserId: {userid}.", highestBidderId);
                return SharedErrors<User>.NotFound;
            }

            userFirstName = user.FirstName;
        }

        return RoomMapper.ToGetAuctionDataResponse(roomWithAuction, userFirstName);
    }
}