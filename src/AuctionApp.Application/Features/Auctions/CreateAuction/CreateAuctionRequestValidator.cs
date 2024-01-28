using AuctionApp.Application.Extensions;
using AuctionApp.Domain.Constants;

using FluentValidation;

namespace AuctionApp.Application.Features.Auction.CreateAuction;

public class CreateAuctionRequestValidator : AbstractValidator<CreateAuctionRequest>
{
    public CreateAuctionRequestValidator()
    {
        RuleFor(x => x.Name)
            .ValidateName();
        RuleFor(x => x.StartingPriceInNaira)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.StartingTime)
            .NotEmpty()
            .GreaterThan(TimeProvider.System.GetUtcNow().DateTime);

        RuleFor(x => x.ClosingTime)
            .NotEmpty()
            .GreaterThan(x => x.StartingTime);
    }
}