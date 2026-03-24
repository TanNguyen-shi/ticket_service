namespace Ticketing.Infrastructure.Configurations;

public class DatabaseOptions
{
    public const string SectionName = "ConnectionStrings";
    public string DefaultConnection { get; set; } = string.Empty;
}