using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Contracts;

public interface IInvoiceService
{
    Task CreateInvoiceAsync(Invoice invoice);
    Task<Invoice?> GetInvoiceAsync(string invoiceId);
    Task<Invoice?> GetInvoiceWithBidAsync(string invoiceId);
}