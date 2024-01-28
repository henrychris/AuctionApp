namespace AuctionApp.Host.Configuration;

public static class ServiceConfiguration
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
    }
}