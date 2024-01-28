using AuctionApp.Domain.Entities.Base;

namespace AuctionApp.Domain.Entities;

public class Invoice : BaseEntity
{
    /// <summary>
    /// The item listed on the invoice.
    /// </summary>
    public required string ItemName { get; set; }

    /// <summary>
    /// The amount listed on the invoice in kobo.
    /// <remarks>
    /// This should match the amount in the bid linked to this invoice.
    /// </remarks>
    /// </summary>
    public required int AmountInKobo { get; set; }

    /// <summary>
    /// The first name of the user who owns the invoice.
    /// </summary>
    public required string UserFirstName { get; set; }

    /// <summary>
    /// The last name of the user who owns the invoice.
    /// </summary>
    public required string UserLastName { get; set; }

    /// <summary>
    /// The email address of the user who owns the invoice.
    /// </summary>
    public required string UserEmail { get; set; }

    public required string BidId { get; set; }
    public Bid Bid { get; set; } = null!;
}