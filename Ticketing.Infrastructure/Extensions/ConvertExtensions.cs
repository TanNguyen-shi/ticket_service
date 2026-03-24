namespace Ticketing.Infrastructure.Extensions;

public static class ConvertExtensions
{
    public static long ToLong(this string? value, long defaultValue = 0)
    {
        return long.TryParse(value, out var result) ? result : defaultValue;
    }

    public static int ToInt(this string? value, int defaultValue = 0)
    {
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    public static bool ToBool(this string? value, bool defaultValue = false)
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        if (bool.TryParse(value, out var boolResult))
            return boolResult;

        if (value == "1")
            return true;

        if (value == "0")
            return false;

        return defaultValue;
    }
}