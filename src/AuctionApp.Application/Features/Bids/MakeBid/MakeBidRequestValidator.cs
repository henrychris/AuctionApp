using FluentValidation;

namespace AuctionApp.Application.Features.Bids.MakeBid;

public class MakeBidRequestValidator : AbstractValidator<MakeBidRequest>
{
    public MakeBidRequestValidator()
    {
        RuleFor(x => x.AmountInNaira)
            .GreaterThan(0)
            .WithMessage("Bid amount must be greater than 0");

        RuleFor(x => x.BiddingRoomId)
            .NotEmpty();
    }
}