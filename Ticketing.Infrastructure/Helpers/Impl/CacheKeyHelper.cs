using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace Ticketing.Infrastructure.Helpers.Impl;

public class CacheKeyHelper : ICacheKeyHelper
{
    public string Build(string prefix, object? payload = null)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Cache key prefix is required.", nameof(prefix));

        var normalizedPayload = NormalizePayload(payload);
        var plainPayload = string.IsNullOrWhiteSpace(normalizedPayload) ? "null" : normalizedPayload;

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainPayload));
        var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

        return $"{prefix}:{hash}";
    }

    private static string NormalizePayload(object? value)
    {
        if (value is null)
            return "null";

        if (value is string s)
            return $"str:{s.Trim().ToLowerInvariant()}";

        if (value is DateTime dt)
            return $"dt:{dt.ToUniversalTime():O}";

        if (value is DateTimeOffset dto)
            return $"dto:{dto.ToUniversalTime():O}";

        if (value is bool b)
            return b ? "bool:true" : "bool:false";

        if (value is byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal)
            return $"num:{Convert.ToString(value, CultureInfo.InvariantCulture)}";

        if (value is IEnumerable<object> objectEnumerable)
            return $"arr:[{string.Join(",", objectEnumerable.Select(NormalizePayload))}]";

        if (value is System.Collections.IEnumerable enumerable and not string)
        {
            var items = new List<string>();
            foreach (var item in enumerable)
            {
                items.Add(NormalizePayload(item));
            }

            return $"arr:[{string.Join(",", items)}]";
        }

        var properties = value.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .Select(p => $"{p.Name.ToLowerInvariant()}={NormalizePayload(p.GetValue(value))}");

        return $"obj:{{{string.Join("|", properties)}}}";
    }
}

