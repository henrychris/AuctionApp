using AuctionApp.Application.Contracts;
using AuctionApp.Application.Extensions;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Auction.CreateAuction;

public class CreateAuctionRequest : IRequest<ErrorOr<CreateAuctionResponse>>
{
    public string Name { get; set; } = string.Empty;
    public decimal StartingPriceInNaira { get; set; }
    public DateTime StartingTime { get; set; }
    public DateTime ClosingTime { get; set; }
}

public class CreateAuctionRequestHandler(
    IAuctionService auctionService,
    ILogger<CreateAuctionRequestHandler> logger,
    IValidator<CreateAuctionRequest> validator) : IRequestHandler<CreateAuctionRequest, ErrorOr<CreateAuctionResponse>>
{
    public async Task<ErrorOr<CreateAuctionResponse>> Handle(CreateAuctionRequest request,
                                                             CancellationToken cancellationToken)
    {
        logger.LogInformation("Trying to create auction with request: {request}.", request);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToErrorList();
            logger.LogError("Validation errors occurred while creating auction with request: {request}. Errors: {errors}.",
                            request,
                            errors);
            return errors;
        }

        var auction = AuctionMapper.CreateAuction(request);
        await auctionService.CreateAuctionAsync(auction);
        logger.LogInformation("Successfully created auction with request: {request}.", request);
        return AuctionMapper.ToCreateAuctionResponse(auction);
    }
}