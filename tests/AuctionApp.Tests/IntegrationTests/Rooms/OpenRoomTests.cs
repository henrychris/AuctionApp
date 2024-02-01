using System.Net;
using System.Net.Http.Json;

using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Features.Auctions.CreateAuction;
using AuctionApp.Application.Features.Rooms.CreateRoom;
using AuctionApp.Application.Features.Rooms.GetSingleRoom;
using AuctionApp.Domain.Constants;
using AuctionApp.Domain.Enums;

using FluentAssertions;

namespace AuctionApp.Tests.IntegrationTests.Rooms;

[TestFixture]
public class OpenRoomTests : IntegrationTest
{
    private readonly CreateAuctionRequest _createAuctionRequest = new CreateAuctionRequest
    {
        Name = "One unit of Gala.",
        StartingTime = DateTime.UtcNow.AddDays(1),
        ClosingTime = DateTime.UtcNow.AddDays(2),
        StartingPriceInNaira = 100
    };

    [Test]
    public async Task OpenAuctionRoom_RoomExists_RoomStatusIsOpen()
    {
        await AuthenticateAsync(Roles.ADMIN);

        var auction = await CreateAuctionAsync(_createAuctionRequest);
        var createRoomRequest = new CreateRoomRequest { AuctionId = auction.AuctionId };

        var response = await TestClient.PostAsJsonAsync("Rooms", createRoomRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var roomRes = await response.Content.ReadFromJsonAsync<ApiResponse<CreateRoomResponse>>();
        var room = roomRes!.Data;
        room.Should().NotBeNull();
        room!.RoomId.Should().NotBeEmpty();

        var openRoomResponse = await TestClient.PostAsJsonAsync($"Rooms/{room.RoomId}/open", new { });
        openRoomResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var postOpenRoom = await TestClient.GetAsync($"Rooms/{room.RoomId}");

        var getRoomRes = await postOpenRoom.Content.ReadFromJsonAsync<ApiResponse<GetRoomResponse>>();
        var getRoom = getRoomRes!.Data;
        getRoom.Should().NotBeNull();
        getRoom!.Status.Should().Be(RoomStatus.Open.ToString());
    }
}