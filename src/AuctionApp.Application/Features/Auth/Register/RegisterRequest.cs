using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using AuctionApp.Application.Extensions;
using AuctionApp.Domain.Constants;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.ServiceErrors;
using AuctionApp.Domain.Settings;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuctionApp.Application.Features.Auth.Register;

public class RegisterRequest : IRequest<ErrorOr<UserAuthResponse>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class RegisterRequestHandler(
    UserManager<User> userManager,
    IOptionsSnapshot<JwtSettings> options,
    ILogger<RegisterRequestHandler> logger,
    IValidator<RegisterRequest> validator) : IRequestHandler<RegisterRequest, ErrorOr<UserAuthResponse>>
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public async Task<ErrorOr<UserAuthResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registration request received for email: {emailAddress}.", request.EmailAddress);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for new user: {emailAddress}", request.EmailAddress);
            return validationResult.ToErrorList();
        }

        var user = await userManager.FindByEmailAsync(request.EmailAddress);
        if (user is not null)
        {
            logger.LogWarning("Duplicate email found during registration: {emailAddress}", request.EmailAddress);
            return Errors.User.DuplicateEmail;
        }

        var newUser = AuthMapper.MapToUser(request);
        var result = await userManager.CreateAsync(newUser, request.Password);
        if (result.Succeeded)
        {
            try
            {
                await userManager.AddToRoleAsync(newUser, newUser.Role);
                logger.LogInformation("User registered successfully: {emailAddress}.", request.EmailAddress);

                return new UserAuthResponse
                {
                    Id = newUser.Id,
                    Role = newUser.Role,
                    AccessToken = GenerateUserToken(newUser.Email!, newUser.Role, newUser.Id)
                };
            }
            catch (Exception)
            {
                logger.LogError(
                    "User registration failed for email: {emailAddress}. Deleting user. Check the exception log.\n",
                    request.EmailAddress);
                await userManager.DeleteAsync(newUser);
                throw;
            }
        }

        var errors = result.Errors
                           .Select(error => Error.Validation("User." + error.Code, error.Description))
                           .ToList();

        logger.LogError(
            "User registration failed for email: {emailAddress}.\nErrors: {errors}", request.EmailAddress,
            string.Join(", ", errors.Select(e => $"{e.Code}: {e.Description}"))
        );

        return errors;
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