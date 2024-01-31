using AuctionApp.Application.Contracts;
using AuctionApp.Domain.Entities;
using AuctionApp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace AuctionApp.Infrastructure.Services;

public class InvoiceService(DataContext context) : IInvoiceService
{
    public async Task CreateInvoiceAsync(Invoice invoice)
    {
        await context.Invoices.AddAsync(invoice);
        await context.SaveChangesAsync();
    }

    public async Task<Invoice?> GetInvoiceAsync(string invoiceId)
    {
        return await context.Invoices.FindAsync(invoiceId);
    }

    public async Task<Invoice?> GetInvoiceWithBidAsync(string invoiceId)
    {
        return await context.Invoices
            .Include(i => i.Bid)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);
    }
}