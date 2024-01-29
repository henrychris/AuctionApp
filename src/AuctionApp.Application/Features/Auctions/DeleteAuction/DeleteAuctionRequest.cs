using AuctionApp.Application.Contracts;
using AuctionApp.Domain.ServiceErrors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Auctions.DeleteAuction
{
    public class DeleteAuctionRequest : IRequest<ErrorOr<Deleted>>
    {
        public required string AuctionId { get; set; }
    }

    public class DeleteAuctionRequestHandler(IAuctionService auctionService, ILogger<DeleteAuctionRequestHandler> logger) : IRequestHandler<DeleteAuctionRequest, ErrorOr<Deleted>>
    {
        public async Task<ErrorOr<Deleted>> Handle(DeleteAuctionRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Received request to delete auction with id {id}...\nRequest: {request}", request.AuctionId, request);

            var auction = await auctionService.GetAuctionByIdAsync(request.AuctionId);
            if (auction == null)
            {
                logger.LogError("Auction with id {id} not found.", request.AuctionId);
                return SharedErrors<Domain.Entities.Auction>.NotFound;
            }

            if (auction.IsInProgress())
            {
                logger.LogError("Auction with id {id} cannot be deleted because it is in progress.", request.AuctionId);
                return Errors.Auction.CannotDeleteInProgressAuction;
            }

            await auctionService.DeleteAuctionAsync(auction);
            logger.LogInformation("Successfully deleted auction with id {id}.", request.AuctionId);

            return Result.Deleted;
        }
    }
}