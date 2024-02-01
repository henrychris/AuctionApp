using System.Net.Http.Headers;
using System.Net.Http.Json;

using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Features.Auctions.CreateAuction;
using AuctionApp.Application.Features.Auctions.GetSingleAuction;
using AuctionApp.Application.Features.Auth;
using AuctionApp.Application.Features.Auth.Register;
using AuctionApp.Application.Features.Invoices.CreateInvoice;
using AuctionApp.Application.Features.Rooms.CreateRoom;
using AuctionApp.Domain.Constants;
using AuctionApp.Host;
using AuctionApp.Infrastructure.Data;

using MassTransit;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionApp.Tests.IntegrationTests;

public class IntegrationTest
{
    protected HttpClient TestClient = null!;
    protected const string AUTH_EMAIL_ADDRESS = "admin@email.com";
    protected const string AUTH_PASSWORD = "testPassword123@";

    [SetUp]
    public void Setup()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // remove dataContext
                    var descriptorsToRemove = services.Where(
                        d => d.ServiceType == typeof(DbContextOptions<DataContext>)).ToList();

                    foreach (var descriptor in descriptorsToRemove)
                    {
                        services.Remove(descriptor);
                    }

                    // replace dataContext with in-memory version
                    services.AddDbContext<DataContext>(options => { options.UseInMemoryDatabase("TestDb"); });
                    services.AddMassTransitTestHarness();
                });
            });
        TestClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost/api/")
        });
    }

    protected async Task AuthenticateAsync(string userRole = Roles.USER)
    {
        TestClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("bearer", await GetJwtAsync(userRole));
    }

    private async Task<string> GetJwtAsync(string userRole = Roles.USER)
    {
        var result = await RegisterAsync(userRole);
        return result?.Data?.AccessToken ?? throw new InvalidOperationException("Registration failed.");
    }

    protected async Task<ApiResponse<UserAuthResponse>?> RegisterAsync(string userRole = Roles.USER)
    {
        var registerResponse = await TestClient.PostAsJsonAsync("Auth/register",
            new RegisterRequest
            {
                FirstName = "test",
                LastName = "user",
                EmailAddress = AUTH_EMAIL_ADDRESS,
                Password = AUTH_PASSWORD,
                Role = userRole
            });

        var result = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<UserAuthResponse>>();
        return result;
    }

    protected async Task<CreateAuctionResponse> CreateAuctionAsync(CreateAuctionRequest createAuctionRequest)
    {
        var response = await TestClient.PostAsJsonAsync("Auctions", createAuctionRequest);
        var auctionRes = await response.Content.ReadFromJsonAsync<ApiResponse<CreateAuctionResponse>>();
        var auction = auctionRes!.Data;
        return auction!;
    }

    protected async Task<GetAuctionResponse> GetAuctionAsync(string auctionId)
    {
        var response = await TestClient.GetAsync($"Auctions/{auctionId}");
        var auctionRes = await response.Content.ReadFromJsonAsync<ApiResponse<GetAuctionResponse>>();
        var auction = auctionRes!.Data;
        return auction!;
    }

    protected async Task<CreateRoomResponse> CreateRoomAsync(CreateRoomRequest createRoomRequest)
    {
        var response = await TestClient.PostAsJsonAsync("Rooms", createRoomRequest);
        var roomRes = await response.Content.ReadFromJsonAsync<ApiResponse<CreateRoomResponse>>();
        var room = roomRes!.Data;
        return room!;
    }
}