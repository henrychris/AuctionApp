using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using AuctionApp.Domain.Constants;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.ServiceErrors;
using AuctionApp.Domain.Settings;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuctionApp.Application.Features.Auth.Login;

public class LoginRequest : IRequest<ErrorOr<UserAuthResponse>>
{
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequestHandler(
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    IOptionsSnapshot<JwtSettings> options,
    ILogger<LoginRequestHandler> logger) : IRequestHandler<LoginRequest, ErrorOr<UserAuthResponse>>
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public async Task<ErrorOr<UserAuthResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.EmailAddress);
        if (user is null)
        {
            logger.LogWarning("Email not found during login: {EmailAddress}.", request.EmailAddress);
            return Errors.Auth.LoginFailed;
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (signInResult.Succeeded)
        {
            logger.LogInformation("User {userId} logged in successfully.", user.Id);
            return new UserAuthResponse
            {
                Id = user.Id,
                Role = user.Role,
                AccessToken = GenerateUserToken(user.Email!, user.Role, user.Id)
            };
        }

        if (signInResult.IsLockedOut)
        {
            logger.LogInformation("User {userId} is locked out. End date: {lockoutEnd}.\n\tRequest: {request}", user.Id,
                user.LockoutEnd,
                JsonSerializer.Serialize(request));
            return Errors.User.IsLockedOut;
        }

        if (signInResult.IsNotAllowed)
        {
            logger.LogInformation("User {userId} is not allowed to access the system.\n\tRequest: {request}", user.Id,
                JsonSerializer.Serialize(request));
            return Errors.User.IsNotAllowed;
        }

        logger.LogError("Login failed for user {userId}.\n\tRequest: {request}", user.Id,
            JsonSerializer.Serialize(request));
        return Errors.Auth.LoginFailed;
    }

    private string GenerateUserToken(string emailAddress, string userRole, string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey!));

        var claims = new List<Claim>
        {
            new(JwtClaims.EMAIL, emailAddress), new(JwtClaims.USER_ID, userId), new(JwtClaims.ROLE, userRole)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtSettings.Audience,
            Issuer = _jwtSettings.Issuer,
            Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_jwtSettings.TokenLifeTimeInHours)),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}