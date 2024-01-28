using AuctionApp.Application.Contracts;
using AuctionApp.Application.Extensions;
using AuctionApp.Domain.Entities;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Rooms.CreateRoom;

public class CreateRoomRequest : IRequest<ErrorOr<CreateRoomResponse>>
{
    public required string AuctionId { get; init; }
}

public class CreateRoomRequestHandler(
    IRoomService roomService,
    ILogger<CreateRoomRequestHandler> logger,
    IValidator<CreateRoomRequest> validator) : IRequestHandler<CreateRoomRequest, ErrorOr<CreateRoomResponse>>
{
    public async Task<ErrorOr<CreateRoomResponse>> Handle(CreateRoomRequest request,
                                                          CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating room for auction {AuctionId}", request.AuctionId);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed. Errors: {ValidationErrors}", validationResult.Errors);
            return validationResult.ToErrorList();
        }

        BiddingRoom room = RoomMapper.CreateRoom(request);
        await roomService.CreateRoomAsync(room);

        logger.LogInformation("Room {RoomId} created for auction {AuctionId}", room.Id, request.AuctionId);
        return RoomMapper.ToCreateRoomResponse(room);
    }
}