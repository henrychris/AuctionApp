using AuctionApp.Application.Contracts;
using AuctionApp.Infrastructure.Services;

namespace AuctionApp.Host.Configuration;

public static class ServiceConfiguration
{
    /// <summary>
    /// Register services in the DI container.
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IAuctionService, AuctionService>();
        services.AddScoped<IBidService, BidService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddSingleton(TimeProvider.System);
    }
}