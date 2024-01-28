using AuctionApp.Application.Contracts;
using AuctionApp.Domain.ServiceErrors;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Auction.GetSingleAuction;

public class GetSingleAuctionRequest : IRequest<ErrorOr<GetAuctionResponse>>
{
    public string AuctionId { get; set; } = string.Empty;
}

public class GetSingleAuctionRequestHandler(
    IAuctionService auctionService,
    ILogger<GetSingleAuctionRequestHandler> logger,
    IValidator<GetSingleAuctionRequest> validator
) : IRequestHandler<GetSingleAuctionRequest, ErrorOr<GetAuctionResponse>>
{
    public async Task<ErrorOr<GetAuctionResponse>> Handle(GetSingleAuctionRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching auction with Id: {id}.", request.AuctionId);
        var auction = await auctionService.GetAuctionByIdAsync(request.AuctionId);
        if (auction is null)
        {
            logger.LogError("Auction with Id: {id} not found.", request.AuctionId);
            return SharedErrors<Domain.Entities.Auction>.NotFound;
        }

        logger.LogInformation("Auction with Id: {id} found.", request.AuctionId);
        return AuctionMapper.ToGetAuctionResponse(auction);
    }
}