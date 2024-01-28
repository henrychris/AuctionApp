using AuctionApp.Application.Extensions;

using FluentValidation;

namespace AuctionApp.Application.Features.Auctions.UpdateAuction
{
    public class UpdateAuctionRequestValidator : AbstractValidator<UpdateAuctionRequest>
    {
        public UpdateAuctionRequestValidator()
        {
            RuleFor(x => x.AuctionId)
            .NotEmpty();

            When(x => x.Name is not null, () =>
            RuleFor(x => x.Name)!
            .ValidateName());

            When(x => x.StartingPriceInNaira is not null, () =>
            RuleFor(x => x.StartingPriceInNaira)
            .NotEmpty()
            .GreaterThanOrEqualTo(0));

            When(x => x.StartingTime is not null, () =>
            RuleFor(x => x.StartingTime)
            .NotEmpty()
            .GreaterThan(TimeProvider.System.GetUtcNow().DateTime));

            When(x => x.ClosingTime is not null, () =>
            RuleFor(x => x.ClosingTime)
            .NotEmpty()
            .GreaterThan(x => x.StartingTime));

        }
    }
}