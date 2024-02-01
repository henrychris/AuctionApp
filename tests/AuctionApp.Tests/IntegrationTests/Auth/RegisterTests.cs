using System.Net;
using System.Net.Http.Json;

using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Features.Auth;
using AuctionApp.Application.Features.Auth.Register;

using Bogus;

using FluentAssertions;

namespace AuctionApp.Tests.IntegrationTests.Auth;

[TestFixture]
public class RegisterTests : IntegrationTest
{
    [Test]
    public async Task Register_ValidRequestBody_ReturnsHttpOK()
    {
        // Arrange
        const string userRole = "User";

        var createUserRequest = new Faker<RegisterRequest>()
                                .CustomInstantiator(f =>
                                    new RegisterRequest
                                    {
                                        FirstName = f.Person.FirstName,
                                        LastName = f.Person.LastName,
                                        EmailAddress = f.Person.Email,
                                        Password = "testPassword12@",
                                        Role = userRole
                                    }).Generate();

        // Act
        var act = await TestClient.PostAsJsonAsync("auth/register", createUserRequest);

        // Assert
        act.EnsureSuccessStatusCode();
        act.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await act.Content.ReadFromJsonAsync<ApiResponse<UserAuthResponse>>();
        response!.Data.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Data!.AccessToken.Should().NotBeNullOrWhiteSpace();
        response.Data.Role.Should().Be(userRole);
        response.Data.Id.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task Register_InvalidRequestBody_ReturnsHttpBadRequest()
    {
        // Arrange
        var createUserRequest = new Faker<RegisterRequest>()
                                .CustomInstantiator(f =>
                                    new RegisterRequest
                                    {
                                        FirstName = f.Person.FirstName,
                                        LastName = f.Person.LastName,
                                        EmailAddress = f.Person.Email,
                                        Password = "testPassword12@"
                                    }).Generate();

        // Act
        var act = await TestClient.PostAsJsonAsync("auth/register", createUserRequest);

        // Assert
        // role is missing, so the request is invalid
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}