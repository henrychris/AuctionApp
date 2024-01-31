using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Features.Payments;

public static class PaymentMapper
{
    public static Payment CreatePayment(string invoiceId)
    {
        return new Payment { InvoiceId = invoiceId };
    }
}