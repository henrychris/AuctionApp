using AuctionApp.Application.Contracts;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.ServiceErrors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Rooms.LeaveRoom;

public class LeaveRoomRequestDto
{
    public string ConnectionId { get; set; } = null!;
}

public class LeaveRoomRequest : IRequest<ErrorOr<Success>>
{
    public required string RoomId { get; init; }
    public required string ConnectionId { get; set; }
}

public class LeaveRoomRequestHandler(
    IRoomService roomService,
    ICurrentUser currentUser,
    ILogger<LeaveRoomRequestHandler> logger) : IRequestHandler<LeaveRoomRequest, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(LeaveRoomRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Leaving room {RoomId}", request.RoomId);
        var room = await roomService.GetRoomAsync(request.RoomId);
        if (room is null)
        {
            logger.LogError("Room {RoomId} not found", request.RoomId);
            return SharedErrors<BiddingRoom>.NotFound;
        }

        if (!room.IsOpen())
        {
            logger.LogCritical("Room {RoomId} is closed. How did they get here?", request.RoomId);
            return Errors.BiddingRoom.Closed;
        }

        await roomService.RemoveUserFromRoom(currentUser.UserId, room.Id, request.ConnectionId);
        logger.LogInformation("User {userId} left room {RoomId}", currentUser.UserId, request.RoomId);
        return Result.Success;
    }
}