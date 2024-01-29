using AuctionApp.Application.Contracts;
using AuctionApp.Application.Extensions;
using AuctionApp.Common;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.ServiceErrors;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Bids.MakeBid;

public class BidRequestDto
{
    public string ConnectionId { get; set; } = null!;
    public int BidAmountInNaira { get; set; }
}

public class MakeBidRequest : IRequest<ErrorOr<MakeBidResponse>>
{
    public required string ConnectionId { get; init; }
    public required int BidAmountInNaira { get; init; }
    public required string RoomId { get; init; }
}

public class MakeBidRequestHandler(
    IBidService bidService,
    IRoomService roomService,
    ICurrentUser currentUser,
    IValidator<MakeBidRequest> validator,
    ILogger<MakeBidRequestHandler> logger)
    : IRequestHandler<MakeBidRequest, ErrorOr<MakeBidResponse>>
{
    public async Task<ErrorOr<MakeBidResponse>> Handle(MakeBidRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogError("Validation failed for {request}. Errors: {validationErrors}", request,
                validationResult.Errors);
            return validationResult.ToErrorList();
        }

        var room = await roomService.GetRoomWithAuctionAsync(request.RoomId);
        if (room is null)
        {
            logger.LogError("Room {roomId} does not exist", request.RoomId);
            return SharedErrors<BiddingRoom>.NotFound;
        }

        if (bidService.IsUserAlreadyHighestBidder(room, currentUser.UserId))
        {
            logger.LogError("User {userId} is already the highest bidder", currentUser.UserId);
            return Errors.Bid.UserAlreadyHighestBidder;
        }

        if (!bidService.IsBidAmountHigherThanCurrentBid(room, request.BidAmountInNaira))
        {
            logger.LogError("Bid amount {amount} is not higher than current bid", request.BidAmountInNaira);
            return Errors.Bid.AmountNotHigherThanCurrentHighestBid;
        }

        var bid = BidMapper.CreateBid(request, currentUser.UserId);
        await bidService.CreateBidAsync(bid);

        room.Auction.HighestBidAmountInKobo = CurrencyConverter.ConvertNairaToKobo(request.BidAmountInNaira);
        room.Auction.HighestBidderId = currentUser.UserId;
        await roomService.UpdateRoomAsync(room);

        await roomService.AnnounceNewHighestBidAsync(room, bid, currentUser.FirstName);
        return BidMapper.CreateBidResponse(bid);
    }
}