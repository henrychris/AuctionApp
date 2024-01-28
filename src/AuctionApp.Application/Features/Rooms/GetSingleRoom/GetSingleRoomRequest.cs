using AuctionApp.Application.Contracts;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.ServiceErrors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Rooms.GetSingleRoom;

public class GetSingleRoomRequest : IRequest<ErrorOr<GetRoomResponse>>
{
    public required string RoomId { get; init; }
}

public class GetSingleRoomRequestHandler(IRoomService roomService, ILogger<GetSingleRoomRequestHandler> logger)
    : IRequestHandler<GetSingleRoomRequest, ErrorOr<GetRoomResponse>>
{
    public async Task<ErrorOr<GetRoomResponse>> Handle(GetSingleRoomRequest request,
                                                       CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting room {RoomId}", request.RoomId);
        
        var room = await roomService.GetRoomAsync(request.RoomId);
        if (room is null)
        {
            logger.LogWarning("Room {RoomId} not found", request.RoomId);
            return SharedErrors<BiddingRoom>.NotFound;
        }
        
        logger.LogInformation("Room {RoomId} found", request.RoomId);
        return RoomMapper.ToGetRoomResponse(room);
    }
}