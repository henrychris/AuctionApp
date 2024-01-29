using AuctionApp.Application.Contracts;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.ServiceErrors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Rooms.JoinRoom;

public class JoinRoomRequestDto
{
    public string ConnectionId { get; init; } = null!;
}

public class JoinRoomRequest : IRequest<ErrorOr<Success>>
{
    public required string RoomId { get; init; }
    public required string ConnectionId { get; init; }
}

public class JoinRoomRequestHandler(
    IRoomService roomService,
    ICurrentUser currentUser,
    ILogger<JoinRoomRequestHandler> logger)
    : IRequestHandler<JoinRoomRequest, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(JoinRoomRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        logger.LogInformation("User: {userId} trying to join room {RoomId}", userId, request.RoomId);
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
        
        await roomService.AddUserToRoom(room.Id, currentUser.FirstName, request.ConnectionId);
        logger.LogInformation("User {userId} joined room {RoomId}", userId, request.RoomId);
        return Result.Success;
    }
}