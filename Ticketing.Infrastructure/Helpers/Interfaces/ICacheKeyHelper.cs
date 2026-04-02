namespace Ticketing.Infrastructure.Helpers.Interfaces;

public interface ICacheKeyHelper
{
    string Build(string prefix, object? payload = null);
}

