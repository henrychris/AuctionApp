using AuctionApp.Application.Contracts;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.Enums;
using AuctionApp.Domain.ServiceErrors;

using MediatR;

namespace AuctionApp.Application.Features.Payments.CompletePayment;

public class CompletePaymentCommand : IRequest<ErrorOr<Success>>
{
    public required string InvoiceId { get; init; }
}

public class CompletePaymentCommandHandler(
    IInvoiceService invoiceService,
    IPaymentService paymentService,
    IRoomService roomService)
    : IRequestHandler<CompletePaymentCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(CompletePaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await invoiceService.GetInvoiceWithBidAsync(request.InvoiceId);
        if (invoice is null)
        {
            return SharedErrors<Invoice>.NotFound;
        }

        var payment = await paymentService.GetPaymentAsync(invoice.Id);
        if (payment is null)
        {
            return SharedErrors<Payment>.NotFound;
        }

        if (payment.IsCompleted)
        {
            return Errors.Payments.AlreadyCompleted;
        }

        var room = await roomService.GetRoomWithAuctionAsync(invoice.Bid.BiddingRoomId);
        if (room is null)
        {
            return SharedErrors<BiddingRoom>.NotFound;
        }
        
        payment.Complete();
        room.Status = RoomStatus.Closed;
        room.Auction.Status = AuctionStatus.Ended;
        
        await roomService.UpdateRoomAsync(room);
        await paymentService.UpdatePaymentAsync(payment);
        return Result.Success;
    }
}