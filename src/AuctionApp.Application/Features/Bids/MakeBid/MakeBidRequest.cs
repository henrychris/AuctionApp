using AuctionApp.Application.Contracts;
using AuctionApp.Application.Extensions;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.ServiceErrors;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Bids.MakeBid
{
    public class MakeBidRequest : IRequest<ErrorOr<MakeBidResponse>>
    {
        public decimal AmountInNaira { get; set; }
        public string BiddingRoomId { get; set; } = null!;
    }

    public class MakeBidRequestHandler(IBidService bidService, IRoomService roomService, ICurrentUser currentUser, ILogger<MakeBidRequestHandler> logger, IValidator<MakeBidRequest> validator) : IRequestHandler<MakeBidRequest, ErrorOr<MakeBidResponse>>
    {
        public async Task<ErrorOr<MakeBidResponse>> Handle(MakeBidRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                logger.LogError("Validation failed for {request}. Errors: {validationErrors}", request, validationResult.Errors);
                return validationResult.ToErrorList();
            }

            var room = await roomService.GetRoomWithAuctionAsync(request.BiddingRoomId);
            if (room is null)
            {
                logger.LogError("Room {roomId} does not exist", request.BiddingRoomId);
                return SharedErrors<BiddingRoom>.NotFound;
            }

            if (bidService.IsUserAlreadyHighestBidder(room, currentUser.UserId))
            {
                logger.LogError("User {userId} is already the highest bidder", currentUser.UserId);
                return Errors.Bid.UserAlreadyHighestBidder;
            }

            if (bidService.IsBidAmountTooLow(request.AmountInNaira))
            {
                logger.LogError("Bid amount {amount} is too low", request.AmountInNaira);
                return Errors.Bid.AmountTooLow;
            }

            if (!bidService.IsBidAmountHigherThanCurrentBid(room, request.AmountInNaira))
            {
                logger.LogError("Bid amount {amount} is not higher than current bid", request.AmountInNaira);
                return Errors.Bid.AmountNotHigherThanCurrentHighestBid;
            }

            var bid = BidMapper.CreateBid(request, currentUser.UserId);
            await bidService.CreateBidAsync(bid);

            // todo: send notification to chat here.

            return BidMapper.CreateBidResponse(bid);
        }
    }
}