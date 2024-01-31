using AuctionApp.Application.Features.Invoices.CreateInvoice;
using AuctionApp.Common;
using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Features.Invoices;

public static class InvoiceMapper
{
    public static CreateInvoiceRequest ToCreateInvoiceRequest(BiddingRoom roomWithAuction, User user, string bidId)
    {
        return new CreateInvoiceRequest
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            ItemName = roomWithAuction.Auction.Name,
            AmountInKobo = roomWithAuction.Auction.HighestBidAmountInKobo,
            BidId = bidId
        };
    }

    public static Invoice CreateInvoice(CreateInvoiceRequest contextMessage)
    {
        return new Invoice
        {
            ItemName = contextMessage.ItemName,
            UserFirstName = contextMessage.FirstName,
            UserLastName = contextMessage.LastName,
            UserEmail = contextMessage.Email,
            AmountInKobo = contextMessage.AmountInKobo,
            BidId = contextMessage.BidId
        };
    }
}