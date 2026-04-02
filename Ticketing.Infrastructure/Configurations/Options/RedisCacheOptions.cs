namespace Ticketing.Infrastructure.Configurations.Options;

public class RedisCacheOptions
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; set; } = "localhost:6379,abortConnect=false";

    public string InstanceName { get; set; } = "ticketing:";

    public int DefaultTtlSeconds { get; set; } = 300;
}

