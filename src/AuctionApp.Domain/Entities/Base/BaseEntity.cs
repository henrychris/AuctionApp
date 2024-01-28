using System.ComponentModel.DataAnnotations;

using AuctionApp.Domain.Constants;

namespace AuctionApp.Domain.Entities.Base;

public abstract class BaseEntity
{
    /// <summary>
    /// The unique identifier for this entity.
    /// </summary>
    [Key, MaxLength(DomainConstants.MAX_ID_LENGTH)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The date this entity was created.
    /// </summary>
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The last time this entity was modified.
    /// </summary>
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
}