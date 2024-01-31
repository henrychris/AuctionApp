using AuctionApp.Domain.Settings;

namespace AuctionApp.Host.Configuration;

public static class SettingsConfiguration
{
    private static void ConfigureSettings<T>(IServiceCollection services, IConfiguration? configuration)
        where T : class, new()
    {
        services.Configure<T>(options => configuration?.GetSection(typeof(T).Name).Bind(options));
    }

    public static void SetupConfigFiles(this IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

        Console.WriteLine($"{configuration.AsEnumerable().Count()} secrets retrieved.");

        ConfigureSettings<DatabaseSettings>(services, configuration);
        ConfigureSettings<JwtSettings>(services, configuration);
        ConfigureSettings<MailSettings>(services, configuration);
        ConfigureSettings<RabbitMqSettings>(services, configuration);
        Console.WriteLine("Secrets have been bound to classes.");
    }
}