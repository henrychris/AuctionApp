using AuctionApp.Application.Contracts;
using AuctionApp.Application.Extensions;
using AuctionApp.Common;
using AuctionApp.Domain.ServiceErrors;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Auctions.UpdateAuction
{
    public class UpdateAuctionRequestDto
    {
        public string? Name { get; set; }
        public decimal? StartingPriceInNaira { get; set; }
        public DateTime? StartingTime { get; set; }
        public DateTime? ClosingTime { get; set; }
    }

    public class UpdateAuctionRequest : IRequest<ErrorOr<Updated>>
    {
        public required string AuctionId { get; set; }
        public required string? Name { get; set; }
        public required decimal? StartingPriceInNaira { get; set; }
        public required DateTime? StartingTime { get; set; }
        public required DateTime? ClosingTime { get; set; }
    }

    public class UpdateAuctionRequestHandler(IAuctionService auctionService, ILogger<UpdateAuctionRequestHandler> logger, IValidator<UpdateAuctionRequest> validator) : IRequestHandler<UpdateAuctionRequest, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(UpdateAuctionRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Updating auction with id {id}...\nRequest: {request}", request.AuctionId, request);

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.ToErrorList();
                logger.LogError("Validation failed for {request}. Errors: {errors}", nameof(UpdateAuctionRequest),
                    errors);
                return errors;
            }

            var auction = await auctionService.GetAuctionByIdAsync(request.AuctionId);
            if (auction == null)
            {
                logger.LogError("Auction with id {id} not found.", request.AuctionId);
                return SharedErrors<Domain.Entities.Auction>.NotFound;
            }

            auction.Name = request.Name ?? auction.Name;
            auction.StartingTime = request.StartingTime ?? auction.StartingTime;
            auction.ClosingTime = request.ClosingTime ?? auction.ClosingTime;

            if (request.StartingPriceInNaira.HasValue)
            {
                auction.StartingPriceInKobo = CurrencyConverter.ConvertNairaToKobo(request.StartingPriceInNaira.Value);
            }

            await auctionService.UpdateAuctionAsync(auction);
            logger.LogInformation("Updated auction with id {id}.", request.AuctionId);
            return Result.Updated;
        }
    }
}