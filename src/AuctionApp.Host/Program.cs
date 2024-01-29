using AuctionApp.Host.Configuration;
using AuctionApp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyOrigin", x =>
    {
        x.WithOrigins("http://127.0.0.1:5500", "null")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials();
    });
});
builder.Services.AddSignalR();

builder.Services.SetupConfigFiles();
builder.Services.SetupDatabase<DataContext>();
builder.Services.SetupControllers();
builder.Services.SetupSwagger();
builder.Services.SetupFilters();
builder.Services.SetupMsIdentity();
builder.Services.SetupAuthentication();
builder.Services.RegisterServices();
builder.Services.SetupJsonOptions();
builder.Services.AddFeatures();

var app = builder.Build();
app.UseCors("AllowMyOrigin");
app.RegisterSwagger();
app.RegisterMiddleware();
await app.SeedDatabase();
app.AddSignalRHubs();
app.Run();

// for integration tests
namespace AuctionApp.Host
{
    public partial class Program;
}