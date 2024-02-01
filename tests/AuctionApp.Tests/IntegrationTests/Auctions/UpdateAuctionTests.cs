using System.Net;
using System.Net.Http.Json;

using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Features.Auctions.CreateAuction;
using AuctionApp.Application.Features.Auctions.UpdateAuction;
using AuctionApp.Domain.Constants;

using FluentAssertions;

namespace AuctionApp.Tests.IntegrationTests.Auctions;

[TestFixture]
public class UpdateAuctionTests : IntegrationTest
{
    [Test]
    public async Task UpdateAuction_ValidRequest_ReturnsSuccess()
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

        var updateAuctionRequest = new UpdateAuctionRequestDto
        {
            Name = "One unit of SuperBite.",
            StartingTime = DateTime.UtcNow.AddDays(1),
            ClosingTime = DateTime.UtcNow.AddDays(2),
            StartingPriceInNaira = 200
        };

        // Act
        var response = await TestClient.PutAsJsonAsync($"Auctions/{createdAuction.AuctionId}", updateAuctionRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var postUpdateAuction = await GetAuctionAsync(createdAuction.AuctionId);
        postUpdateAuction.Should().NotBeNull();
        postUpdateAuction!.AuctionId.Should().Be(createdAuction.AuctionId);

        // untouched items should remain the same
        postUpdateAuction.HighestBidAmountInNaira.Should()
                         .Be(createAuctionRequest.StartingPriceInNaira);
        postUpdateAuction.StartingTime.Should().BeCloseTo(createAuctionRequest.StartingTime, TimeSpan.FromSeconds(10));
        postUpdateAuction.ClosingTime.Should().BeCloseTo(createAuctionRequest.ClosingTime, TimeSpan.FromSeconds(10));

        // updated items should be different
        postUpdateAuction.Name.Should().Be(updateAuctionRequest.Name);
        postUpdateAuction.StartingPriceInNaira.Should()
                         .Be(updateAuctionRequest.StartingPriceInNaira);
    }

    [Test]
    public async Task UpdateAuction_AuctionDoesNotExist_ReturnsNotFound()
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

        var updateAuctionRequest = new UpdateAuctionRequestDto
        {
            Name = "One unit of SuperBite.",
            StartingTime = DateTime.UtcNow.AddDays(1),
            ClosingTime = DateTime.UtcNow.AddDays(2),
            StartingPriceInNaira = 200
        };

        // Act
        var response = await TestClient.PutAsJsonAsync($"Auctions/{Guid.NewGuid()}", updateAuctionRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}