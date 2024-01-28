namespace AuctionApp.Application.Features.Rooms.GetSingleRoom;

public class GetRoomResponse
{
    public required string Status { get; set; }
    public required string AuctionId { get; set; }
    public required string RoomId { get; set; }
}