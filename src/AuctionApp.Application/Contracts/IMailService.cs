using AuctionApp.Domain.Entities.Notifications;

namespace AuctionApp.Application.Contracts;

public interface IMailService
{
    Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    public string LoadTemplate(string pathToTemplate);
}