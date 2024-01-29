using AuctionApp.Application.Contracts;
using AuctionApp.Common;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.ServiceErrors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Rooms.OpenRoom;

public class OpenRoomRequest : IRequest<ErrorOr<Success>>
{
    public required string RoomId { get; set; }
}

public class OpenRoomAuctionRequestHandler(ILogger<OpenRoomRequest> logger, IRoomService roomService)
    : IRequestHandler<OpenRoomRequest, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(OpenRoomRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Opening room {RoomId}", request.RoomId);
        var room = await roomService.GetRoomWithAuctionAsync(request.RoomId);
        if (room is null)
        {
            logger.LogError("Room {RoomId} not found", request.RoomId);
            return SharedErrors<BiddingRoom>.NotFound;
        }

        room.Open();
        await roomService.UpdateRoomAsync(room);
        logger.LogInformation("Room {RoomId} opened", request.RoomId);
        return Result.Success;
    }
}