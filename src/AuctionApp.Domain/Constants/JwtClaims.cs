using System.Security.Claims;

namespace AuctionApp.Domain.Constants;

public class JwtClaims
{
    public const string EMAIL = "Email";
    public const string USER_ID = ClaimTypes.NameIdentifier;
    public const string ROLE = "Role";
    public const string FIRST_NAME = "FirstName";
}