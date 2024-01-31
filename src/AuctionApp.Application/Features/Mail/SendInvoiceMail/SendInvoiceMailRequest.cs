using System.Globalization;

using AuctionApp.Application.Contracts;
using AuctionApp.Common;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.Entities.Notifications;

using MassTransit;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Mail.SendInvoiceMail;

public class SendInvoiceMailRequest
{
    public required Invoice InvoiceDetails { get; set; }
}

public class SendInvoiceMailConsumer(IMailService mailService, ILogger<SendInvoiceMailConsumer> logger) : IConsumer<SendInvoiceMailRequest>
{
    public async Task Consume(ConsumeContext<SendInvoiceMailRequest> context)
    {
        var invoiceDetails = context.Message.InvoiceDetails;
        var amountInNaira = CurrencyConverter.ConvertKoboToNaira(invoiceDetails.AmountInKobo);

        List<string> toEmailAddress = [invoiceDetails.UserEmail];

        var emailTemplate = mailService.LoadTemplate("invoice");
        emailTemplate = emailTemplate
                        .Replace("{FirstName}", invoiceDetails.UserFirstName)
                        .Replace("{LastName}", invoiceDetails.UserLastName)
                        .Replace("{ItemName}", invoiceDetails.ItemName)
                        .Replace("{AmountInNaira}", amountInNaira.ToString(CultureInfo.InvariantCulture))
                        .Replace("{InvoiceId}", invoiceDetails.Id);

        logger.LogInformation("Sending invoice email to {emailAddress}", invoiceDetails.UserEmail);
        await mailService.SendAsync(
            new MailData { Body = emailTemplate, Subject = "Your invoice.", To = toEmailAddress, Attachments = null },
            new CancellationToken());
    }
}