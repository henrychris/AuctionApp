using System.Text;

using AuctionApp.Domain.Constants;
using AuctionApp.Domain.Settings;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuctionApp.Host.Configuration;

public static class AuthConfiguration
{
    public static void SetupAuthentication(this IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var jwtSettings = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<JwtSettings>>().Value;

        services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(
                    x =>
                    {
                        x.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidAudience = jwtSettings.Audience ??
                                            throw new InvalidOperationException("Audience is null!"),
                            ValidIssuer = jwtSettings.Issuer ??
                                          throw new InvalidOperationException("Security Key is null!"),
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey ??
                                throw new InvalidOperationException("Security Key is null!"))),
                            ValidateAudience = true,
                            ValidateIssuer = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            RoleClaimType = JwtClaims.ROLE
                        };
                    });
        services.AddAuthorization();
    }
}