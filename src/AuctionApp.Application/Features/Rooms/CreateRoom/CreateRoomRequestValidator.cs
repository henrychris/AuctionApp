using FluentValidation;

namespace AuctionApp.Application.Features.Rooms.CreateRoom;

public class CreateRoomRequestValidator : AbstractValidator<CreateRoomRequest>
{
    public CreateRoomRequestValidator()
    {
        RuleFor(x => x.AuctionId)
            .NotEmpty()
            .WithMessage("Auction ID is required.");
    }
}