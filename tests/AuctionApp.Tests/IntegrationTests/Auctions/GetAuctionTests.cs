using System.Net;
using System.Net.Http.Json;

using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Features.Auctions.CreateAuction;
using AuctionApp.Application.Features.Auctions.GetSingleAuction;
using AuctionApp.Domain.Constants;

using FluentAssertions;

namespace AuctionApp.Tests.IntegrationTests.Auctions;

[TestFixture]
public class GetAuctionTests : IntegrationTest
{
    [Test]
    public async Task GetAuction_ValidRequest_ReturnsSuccess()
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

        var createdAuction = await CreateAuctionAsync(createAuctionRequest);
        createdAuction.AuctionId.Should().NotBeEmpty();

        // Act
        var response = await TestClient.GetAsync($"Auctions/{createdAuction.AuctionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var auctionRes2 = await response.Content.ReadFromJsonAsync<ApiResponse<GetAuctionResponse>>();
        var auction2 = auctionRes2!.Data;
        auction2.Should().NotBeNull();
        auction2!.AuctionId.Should().Be(createdAuction.AuctionId);
        auction2.Name.Should().Be(createAuctionRequest.Name);
        auction2.StartingTime.Should().Be(createAuctionRequest.StartingTime);
        auction2.ClosingTime.Should().Be(createAuctionRequest.ClosingTime);
        auction2.StartingPriceInNaira.Should()
                .Be(createAuctionRequest.StartingPriceInNaira);
        auction2.HighestBidAmountInNaira.Should()
                .Be(createAuctionRequest.StartingPriceInNaira);
    }

    [Test]
    public async Task GetAuction_AuctionDoesNotExist_ReturnsNotFound()
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

        var createdAuction = await CreateAuctionAsync(createAuctionRequest);
        createdAuction.AuctionId.Should().NotBeEmpty();

        // Act
        var response = await TestClient.GetAsync($"Auctions/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}