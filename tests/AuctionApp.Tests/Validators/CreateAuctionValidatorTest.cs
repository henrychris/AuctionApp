using AuctionApp.Application.Features.Auctions.CreateAuction;

using FluentValidation.TestHelper;

namespace AuctionApp.Tests.Validators;

[TestFixture]
public class CreateAuctionValidatorTest
{
    private CreateAuctionRequestValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new CreateAuctionRequestValidator();
    }

    [Test]
    public void CreateAuction_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var createAuctionRequest = new CreateAuctionRequest
        {
            Name = "One unit of Gala.",
            StartingTime = DateTime.UtcNow.AddDays(1),
            ClosingTime = DateTime.UtcNow.AddDays(2),
            StartingPriceInNaira = 100
        };

        // Act
        var result = _validator.TestValidate(createAuctionRequest);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void CreateAuction_InvalidStartingPrice_ReturnsValidationError()
    {
        // Arrange
        var createAuctionRequest = new CreateAuctionRequest
        {
            Name = "One unit of Gala.",
            StartingTime = DateTime.UtcNow.AddDays(1),
            ClosingTime = DateTime.UtcNow.AddDays(2),
            StartingPriceInNaira = 0
        };

        // Act
        var result = _validator.TestValidate(createAuctionRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartingPriceInNaira);
    }

    [Test]
    public void CreateAuction_InvalidStartingTime_ReturnsValidationError()
    {
        // Arrange
        var createAuctionRequest = new CreateAuctionRequest
        {
            Name = "One unit of Gala.",
            StartingTime = DateTime.UtcNow.AddDays(-1),
            ClosingTime = DateTime.UtcNow.AddDays(2),
            StartingPriceInNaira = 100
        };

        // Act
        var result = _validator.TestValidate(createAuctionRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartingTime);
    }

    [Test]
    public void CreateAuction_InvalidClosingTime_ReturnsValidationError()
    {
        // Arrange
        var createAuctionRequest = new CreateAuctionRequest
        {
            Name = "One unit of Gala.",
            StartingTime = DateTime.UtcNow.AddDays(1),
            ClosingTime = DateTime.UtcNow.AddDays(-2),
            StartingPriceInNaira = 100
        };

        // Act
        var result = _validator.TestValidate(createAuctionRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ClosingTime);
    }

    [Test]
    public void CreateAuction_InvalidName_ReturnsValidationError()
    {
        // Arrange
        var createAuctionRequest = new CreateAuctionRequest
        {
            Name = "",
            StartingTime = DateTime.UtcNow.AddDays(1),
            ClosingTime = DateTime.UtcNow.AddDays(2),
            StartingPriceInNaira = 100
        };

        // Act
        var result = _validator.TestValidate(createAuctionRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}