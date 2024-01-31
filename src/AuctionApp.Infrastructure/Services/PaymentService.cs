using AuctionApp.Application.Contracts;
using AuctionApp.Domain.Entities;
using AuctionApp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace AuctionApp.Infrastructure.Services;

public class PaymentService(DataContext context) : IPaymentService
{
    public async Task CreatePaymentAsync(Payment payment)
    {
        await context.Payments.AddAsync(payment);
        await context.SaveChangesAsync();
    }

    public async Task<Payment?> GetPaymentAsync(string invoiceId)
    {
        return await context.Payments.FirstOrDefaultAsync(p => p.InvoiceId == invoiceId);
    }

    public async Task UpdatePaymentAsync(Payment payment)
    {
        context.Payments.Update(payment);
        await context.SaveChangesAsync();
    }
}