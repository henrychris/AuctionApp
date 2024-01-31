using AuctionApp.Application.Contracts;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.Enums;
using AuctionApp.Domain.ServiceErrors;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Rooms.EndRoomAuction;

public class EndRoomAuctionRequest : IRequest<ErrorOr<Success>>
{
    public required string RoomId { get; set; }
}

public class EndRoomAuctionRequestHandler(
    IRoomService roomService,
    UserManager<User> userManager,
    ILogger<EndRoomAuctionRequest> logger)
    : IRequestHandler<EndRoomAuctionRequest, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(EndRoomAuctionRequest request, CancellationToken cancellationToken)
    {
        var roomWithAuction = await roomService.GetRoomWithAuctionAndBidsAsync(request.RoomId);
        if (roomWithAuction is null)
        {
            logger.LogCritical("The bidding room does not exist.");
            return SharedErrors<BiddingRoom>.NotFound;
        }

        if (roomWithAuction.Bids.Count == 0)
        {
            return Errors.BiddingRoom.NoBidsYet;
        }

        if (!roomWithAuction.Auction.IsInProgress())
        {
            logger.LogCritical("Can't end an auction that hasn't started.");
            return Errors.Auction.NotStartedYet;
        }

        roomWithAuction.Auction.Status = AuctionStatus.AwaitingPayment;
        roomWithAuction.Status = RoomStatus.Closed;
        
        var highestBid = roomWithAuction.Auction.HighestBidAmountInKobo;
        var highestBidderId =
            roomWithAuction.Auction.HighestBidderId!;
        var user = await userManager.FindByIdAsync(highestBidderId);

        await roomService.UpdateRoomAsync(roomWithAuction);
        await roomService.AnnounceEndOfAuction(new EndAuctionDto(request.RoomId,
            user!.FirstName, highestBid));
        await roomService.KickAllUsersFromGroup(request.RoomId);

        // todo: process invoice. need to get all details then queue message
        return Result.Success;
    }
}

// queue invoice creation and email sending