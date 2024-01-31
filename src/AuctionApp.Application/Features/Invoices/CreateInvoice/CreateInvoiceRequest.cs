using AuctionApp.Application.Contracts;
using AuctionApp.Application.Features.Mail.SendInvoiceMail;
using AuctionApp.Application.Features.Payments;
using AuctionApp.Domain.Entities;

using MassTransit;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Invoices.CreateInvoice;

/// <summary>
/// This is sent by massTransit to the queue
/// </summary>
public class CreateInvoiceRequest
{
    public required string ItemName { get; set; }
    public required int AmountInKobo { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string BidId { get; set; }
}

/// <summary>
/// This subscribes to the CreateInvoiceMessageQueue and receives any messages.
/// </summary>
public class CreateInvoiceConsumer(
    IInvoiceService invoiceService,
    IPaymentService paymentService,
    IBus bus,
    ILogger<CreateInvoiceConsumer> logger)
    : IConsumer<CreateInvoiceRequest>
{
    public async Task Consume(ConsumeContext<CreateInvoiceRequest> context)
    {
        var invoice = InvoiceMapper.CreateInvoice(context.Message);
        await invoiceService.CreateInvoiceAsync(invoice);
        logger.LogInformation("Created invoice {id} for item {itemName}.", invoice.Id, invoice.ItemName);

        var payment = PaymentMapper.CreatePayment(invoice.Id);
        await paymentService.CreatePaymentAsync(payment);
        logger.LogInformation("Created incomplete payment for invoice {id}", invoice.Id);

        // queue invoice email
        await bus.Publish(new SendInvoiceMailRequest { InvoiceDetails = invoice });
        logger.LogInformation("Queued invoice email for invoice {id}", invoice.Id);
    }
}