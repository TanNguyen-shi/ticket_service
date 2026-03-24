using System.Dynamic;
using System.Reflection;
using Ticketing.Infrastructure.Attributes;

namespace Ticketing.Infrastructure.Helpers
{
    public static class ParamExtensions
    {
        public static IDictionary<string, object> ToDictionary<T>(
            T request,
            string[] excludedKeys,
            params (string key, object value)[] additionalParams)
        {
            var dict = new ExpandoObject() as IDictionary<string, object>;
            excludedKeys ??= Array.Empty<string>();
            if (request is not null)
            {
                foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (excludedKeys.Contains(prop.Name))
                        continue;

                    if (Attribute.IsDefined(prop, typeof(DbParamIgnoreAttribute)))
                        continue;

                    var value = prop.GetValue(request);
                    if (value is Enum)
                        value = Convert.ToInt32(value);
                    dict[prop.Name] = value ?? DBNull.Value;
                }
            }

            foreach (var (key, value) in additionalParams ?? Array.Empty<(string, object)>())
            {
                dict[key] = value ?? DBNull.Value;
            }

            return dict;
        }

        public static IDictionary<string, object?> ToDictionary<T>(
            T request,
            long? userLogin,
            string[]? excludedKeys = null,
            params (string key, object? value)[] additionalParams)
        {
            additionalParams ??= Array.Empty<(string key, object? value)>();

            if (userLogin > 0)
            {
                additionalParams = new[] { ("userlogin", (object?)userLogin) }
                    .Concat(additionalParams)
                    .ToArray();
            }

            return ToDictionary(request, excludedKeys, additionalParams);
        }
    }
}