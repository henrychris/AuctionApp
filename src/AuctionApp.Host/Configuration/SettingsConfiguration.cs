using AuctionApp.Domain.Settings;

namespace AuctionApp.Host.Configuration;

public static class SettingsConfiguration
{
    /// <summary>
    /// Configures a settings class with values from the application's configuration.
    /// </summary>
    /// <remarks>
    /// The settings class' name must match the name of the section in the configuration.
    /// </remarks>
    /// <typeparam name="T">The type of the settings class.</typeparam>
    /// <param name="services">The IServiceCollection to add the settings to.</param>
    /// <param name="configuration">The application's configuration.</param>
    private static void ConfigureSettings<T>(IServiceCollection services, IConfiguration? configuration)
        where T : class, new()
    {
        services.Configure<T>(options => configuration?.GetSection(typeof(T).Name).Bind(options));
    }

    /// <summary>
    /// Sets up the application's configuration files and binds the settings classes to the configuration.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the settings to.</param>
    public static void SetupConfigFiles(this IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            // .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Console.WriteLine($"{configuration.AsEnumerable().Count()} secrets retrieved.");

        ConfigureSettings<DatabaseSettings>(services, configuration);
        ConfigureSettings<JwtSettings>(services, configuration);
        ConfigureSettings<MailSettings>(services, configuration);
        ConfigureSettings<RabbitMqSettings>(services, configuration);
        Console.WriteLine("Secrets have been bound to classes.");
    }
}