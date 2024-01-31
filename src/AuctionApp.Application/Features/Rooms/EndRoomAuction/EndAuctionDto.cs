namespace AuctionApp.Application.Features.Rooms.EndRoomAuction;

public class EndAuctionDto
{
    public EndAuctionDto(string roomId, string winnerFirstName, int bidAmountInKobo)
    {
        RoomId = roomId;
        WinnerFirstName = winnerFirstName;
        BidAmountInKobo = bidAmountInKobo;
    }

    public string RoomId { get; private set; }
    public string WinnerFirstName { get; private set; }
    public int BidAmountInKobo { get; private set; }
}