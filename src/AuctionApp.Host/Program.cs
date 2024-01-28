using AuctionApp.Host.Configuration;
using AuctionApp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
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
app.RegisterSwagger();
app.RegisterMiddleware();
await app.SeedDatabase();
app.Run();

// for integration tests
public partial class Program;