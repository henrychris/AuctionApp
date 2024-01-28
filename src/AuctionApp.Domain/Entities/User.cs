using System.ComponentModel.DataAnnotations;

using AuctionApp.Domain.Constants;

using Microsoft.AspNetCore.Identity;

namespace AuctionApp.Domain.Entities;

public class User : IdentityUser
{
    [MaxLength(DomainConstants.MAX_ID_LENGTH)]
    [Key]
    public override string Id { get; set; } = Guid.NewGuid().ToString();

    [MaxLength(DomainConstants.MAX_NAME_LENGTH)]
    public required string FirstName { get; set; }

    [MaxLength(DomainConstants.MAX_NAME_LENGTH)]
    public required string LastName { get; set; }

    [MaxLength(DomainConstants.MAX_ENUM_LENGTH)]
    public required string Role { get; set; }
}