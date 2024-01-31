using AuctionApp.Application.Contracts;
using AuctionApp.Application.Features.Invoices;
using AuctionApp.Application.Features.Invoices.CreateInvoice;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.Enums;
using AuctionApp.Domain.ServiceErrors;

using MassTransit;

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
    IBus bus,
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

        // get bid details
        var highestBidAmountInKobo = roomWithAuction.Auction.HighestBidAmountInKobo;
        var highestBidderId = roomWithAuction.Auction.HighestBidderId!;
        var highestBid = roomWithAuction.Bids
                                        .FirstOrDefault(x => x.UserId == highestBidderId &&
                                                             x.AmountInKobo == highestBidAmountInKobo)!;

        var user = await userManager.FindByIdAsync(highestBidderId);

        // update the room and announce the end of the auction
        await roomService.UpdateRoomAsync(roomWithAuction);
        await roomService.AnnounceEndOfAuction(new EndAuctionDto(request.RoomId,
            user!.FirstName, highestBidAmountInKobo));

        // kick all users from the group
        await roomService.KickAllUsersFromGroup(request.RoomId);

        logger.LogInformation("Ended auction for room {roomId}.", request.RoomId);
        // process invoice
        await bus.Publish(InvoiceMapper.ToCreateInvoiceRequest(roomWithAuction, user, highestBid.Id),
            CancellationToken.None);
        logger.LogInformation("Published invoice creation request for room {roomId}.", request.RoomId);
        return Result.Success;
    }
}