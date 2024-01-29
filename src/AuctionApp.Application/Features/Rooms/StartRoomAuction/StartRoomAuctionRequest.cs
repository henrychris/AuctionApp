using AuctionApp.Application.Contracts;
using AuctionApp.Common;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.ServiceErrors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Rooms.StartRoomAuction;

public class StartRoomAuctionRequest : IRequest<ErrorOr<Success>>
{
    public required string RoomId { get; set; }
}

public class StartRoomAuctionRequestHandler(ILogger<StartRoomAuctionRequestHandler> logger, IRoomService roomService)
    : IRequestHandler<StartRoomAuctionRequest, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(StartRoomAuctionRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting auction for room {RoomId}", request.RoomId);
        var room = await roomService.GetRoomWithAuctionAsync(request.RoomId);
        if (room is null)
        {
            logger.LogError("Room {RoomId} not found", request.RoomId);
            return SharedErrors<BiddingRoom>.NotFound;
        }

        if (room.Auction.IsInProgress())
        {
            logger.LogInformation("Auction for room {roomId} is already in progress.", request.RoomId);
            return Errors.Auction.AlreadyStarted;
        }

        if (!room.IsOpen())
        {
            logger.LogInformation("Room {RoomId} is not open", request.RoomId);
            return Errors.BiddingRoom.NotOpen;
        }

        room.Auction.Start();
        await roomService.UpdateRoomAsync(room);
        logger.LogInformation("Auction for room {RoomId} started", request.RoomId);

        await roomService.SendAuctionStartMessageToClientsAsync(
            room.Id,
            room.Auction.Name,
            CurrencyConverter.ConvertKoboToNaira(room.Auction.StartingPriceInKobo)
        );
        return Result.Success;
    }
}