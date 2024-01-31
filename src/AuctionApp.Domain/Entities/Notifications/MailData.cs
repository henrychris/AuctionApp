namespace AuctionApp.Domain.Entities.Notifications;

public class MailData
{
    // Receiver
    public List<string>? To { get; set; }

    // Content
    public required string Subject { get; set; }
    public required string Body { get; set; }

    public IFormFileCollection? Attachments { get; set; }
}