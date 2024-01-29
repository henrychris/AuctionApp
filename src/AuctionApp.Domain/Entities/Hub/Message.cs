namespace AuctionApp.Domain.Entities.Hub;

public class Message
{
    public Message(string userName, string content)
    {
        Content = content;
        UserName = userName;
    }

    public string Content { get; set; }
    public string UserName { get; set; }
}