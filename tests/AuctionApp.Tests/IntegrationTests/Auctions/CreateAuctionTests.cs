using System.Net;
using System.Net.Http.Json;

using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Features.Auctions.CreateAuction;
using AuctionApp.Domain.Constants;

using FluentAssertions;

namespace AuctionApp.Tests.IntegrationTests.Auctions;

[TestFixture]
public class CreateAuctionTests : IntegrationTest
{
    [Test]
    public async Task CreateAuction_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsync(Roles.ADMIN);
        var createAuctionRequest = new CreateAuctionRequest
        {
            Name = "One unit of Gala.",
            StartingTime = DateTime.UtcNow.AddDays(1),
            ClosingTime = DateTime.UtcNow.AddDays(2),
            StartingPriceInNaira = 100
        };

        // Act
        var response = await TestClient.PostAsJsonAsync("Auctions", createAuctionRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var auctionRes = await response.Content.ReadFromJsonAsync<ApiResponse<CreateAuctionResponse>>();
        var auction = auctionRes!.Data;
        auction.Should().NotBeNull();
        auction!.AuctionId.Should().NotBeEmpty();
        // auction.Name.Should().Be(createAuctionRequest.Name);
        // auction.StartingTime.Should().Be(createAuctionRequest.StartingTime);
        // auction.ClosingTime.Should().Be(createAuctionRequest.ClosingTime);
        // auction.StartingPriceInKobo.Should()
        //        .Be(CurrencyConverter.ConvertNairaToKobo(createAuctionRequest.StartingPriceInNaira));
        // auction.HighestBidAmountInKobo.Should()
        //        .Be(CurrencyConverter.ConvertNairaToKobo(createAuctionRequest.StartingPriceInNaira));
    }

    [Test]
    public async Task CreateAuction_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        await AuthenticateAsync();
        var createAuctionRequest = new CreateAuctionRequest
        {
            Name = "One unit of Gala.",
            StartingTime = DateTime.UtcNow.AddDays(1),
            ClosingTime = DateTime.UtcNow.AddDays(2),
            StartingPriceInNaira = 100
        };

        // Act
        var response = await TestClient.PostAsJsonAsync("Auctions", createAuctionRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task CreateAuction_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsync(Roles.ADMIN);
        var createAuctionRequest = new CreateAuctionRequest
        {
            Name = "One unit of Gala.",
            StartingTime = DateTime.UtcNow.AddDays(-1),
            ClosingTime = DateTime.UtcNow.AddDays(2),
            StartingPriceInNaira = 100
        };

        // Act
        var response = await TestClient.PostAsJsonAsync("Auctions", createAuctionRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}