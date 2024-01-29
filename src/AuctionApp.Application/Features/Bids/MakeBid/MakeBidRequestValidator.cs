using FluentValidation;

namespace AuctionApp.Application.Features.Bids.MakeBid;

public class MakeBidRequestValidator : AbstractValidator<MakeBidRequest>
{
    public MakeBidRequestValidator()
    {
        RuleFor(x => x.ConnectionId).NotEmpty();
        RuleFor(x => x.BidAmountInNaira).GreaterThan(0);
        RuleFor(x => x.RoomId).NotEmpty();
    }
}