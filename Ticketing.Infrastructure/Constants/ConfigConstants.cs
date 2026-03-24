using Microsoft.Extensions.Configuration;

namespace Ticketing.Infrastructure.Constants;

public static class ConfigHelper
{
    private static readonly Lazy<IConfigurationRoot> _configuration = new(() =>
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();
    });

    private static IConfiguration Configuration => _configuration.Value;

    public static string EnvironmentName =>
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

    public static bool IsDev =>
        EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase);

    public static string Get(string key) => Configuration[key] ?? string.Empty;

    public static T GetValue<T>(string key, T defaultValue) =>
        Configuration.GetValue(key, defaultValue);
}