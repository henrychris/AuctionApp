using AuctionApp.Application.Features.Rooms.CreateRoom;
using AuctionApp.Application.Features.Rooms.GetSingleRoom;
using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Features.Rooms;

public static class RoomMapper
{
    public static BiddingRoom CreateRoom(CreateRoomRequest request)
    {
        return new BiddingRoom { AuctionId = request.AuctionId };
    }

    public static CreateRoomResponse ToCreateRoomResponse(BiddingRoom room)
    {
        return new CreateRoomResponse { RoomId = room.Id };
    }

    public static GetRoomResponse ToGetRoomResponse(BiddingRoom room)
    {
        return new GetRoomResponse { RoomId = room.Id, AuctionId = room.AuctionId, Status = room.Status.ToString() };
    }
}