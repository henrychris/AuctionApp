using System.Net;

using AuctionApp.Application.Features.Auctions.CreateAuction;
using AuctionApp.Domain.Constants;

using FluentAssertions;

namespace AuctionApp.Tests.IntegrationTests.Auctions;

[TestFixture]
public class DeleteAuctionTest : IntegrationTest
{
    [Test]
    public async Task DeleteAuction_ValidRequest_ReturnsSuccess()
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
        var response = await TestClient.DeleteAsync($"Auctions/{createdAuction.AuctionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}