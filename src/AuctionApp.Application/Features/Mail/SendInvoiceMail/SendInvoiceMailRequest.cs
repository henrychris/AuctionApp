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
    private const string htmlTemplate = @"
    <!DOCTYPE html>
<html lang='en'>
  <head>
    <meta charset='UTF-8' />
    <meta http-equiv='X-UA-Compatible' content='IE=edge' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>Invoice</title>
    <style>
      body {
        font-family: 'Arial', sans-serif;
        margin: 0;
        padding: 0;
        background-color: #f4f4f4;
      }

      .container {
        max-width: 600px;
        margin: 20px auto;
        background-color: #fff;
        padding: 20px;
        border-radius: 8px;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
      }

      h1 {
        color: #333;
      }

      p {
        color: #555;
      }

      .invoice-details {
        margin-top: 20px;
      }

      .payment-link {
        display: inline-block;
        margin-top: 20px;
        padding: 10px 20px;
        background-color: #3498db;
        color: #fff;
        text-decoration: none;
        border-radius: 5px;
      }

      .footer {
        margin-top: 20px;
        color: #777;
        font-size: 12px;
      }
    </style>
  </head>
  <body>
    <div class='container'>
      <h1>Invoice</h1>
      <p>Dear<strong> {FirstName} {LastName}</strong>,</p>
      <p>You have an outstanding invoice for the following item:</p>
      <div class='invoice-details'>
        <p><strong> Item:</strong> {ItemName}</p>
        <p><strong> Amount:</strong> {AmountInNaira} NGN</p>
      </div>
      <p>Please click the link below to make the payment:</p>
      <a
        href='http://localhost:3000/pages/payment.html?invoiceId={InvoiceId}'
        class='payment-link'
      >
        Make Payment
      </a>
      <div class='footer'>
        <p>Thank you for your business!</p>
      </div>
    </div>
  </body>
</html>
    ";

    public async Task Consume(ConsumeContext<SendInvoiceMailRequest> context)
    {
        var invoiceDetails = context.Message.InvoiceDetails;
        var amountInNaira = CurrencyConverter.ConvertKoboToNaira(invoiceDetails.AmountInKobo);

        List<string> toEmailAddress = [invoiceDetails.UserEmail];

        var emailTemplate = htmlTemplate;
        emailTemplate = emailTemplate
                        .Replace("{FirstName}", invoiceDetails.UserFirstName)
                        .Replace("{LastName}", invoiceDetails.UserLastName)
                        .Replace("{ItemName}", invoiceDetails.ItemName)
                        .Replace("{AmountInNaira}", amountInNaira.ToString(CultureInfo.InvariantCulture))
                        .Replace("{InvoiceId}", invoiceDetails.Id);

        logger.LogInformation("Sending invoice email to {emailAddress}", invoiceDetails.UserEmail);
        var result = await mailService.SendAsync(
            new MailData { Body = emailTemplate, Subject = "Your invoice.", To = toEmailAddress, Attachments = null },
            new CancellationToken());

        if (!result)
        {
            // masstransit requeues on exceptions.
            throw new Exception("Mail failed to send. Requeueing...");
        }
    }
}