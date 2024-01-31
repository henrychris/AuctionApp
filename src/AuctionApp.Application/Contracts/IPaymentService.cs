using AuctionApp.Domain.Entities;

namespace AuctionApp.Application.Contracts;

public interface IPaymentService
{
    Task CreatePaymentAsync(Payment payment);
    Task<Payment?> GetPaymentAsync(string invoiceId);
    Task UpdatePaymentAsync(Payment payment);
}