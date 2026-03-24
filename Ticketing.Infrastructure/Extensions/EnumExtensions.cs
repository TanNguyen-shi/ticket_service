using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Ticketing.Infrastructure.Extensions
{
	public static class EnumExtensions
	{ 
        public static string GetDisplayName(this Enum enumValue, string invalidText = "Không hợp lệ")
        {
            if (enumValue is null) return invalidText;

            var type = enumValue.GetType();
            var name = enumValue.ToString();

            // Lấy đúng member theo tên
            var member = type.GetMember(name).FirstOrDefault();

            // Nếu không tìm thấy member và cũng không phải giá trị hợp lệ -> không hợp lệ
            if (member == null && !Enum.IsDefined(type, enumValue))
                return invalidText;

            // Nếu có Display thì dùng, nếu không thì trả về tên code
            var display = member?.GetCustomAttribute<DisplayAttribute>()?.GetName();
            return string.IsNullOrWhiteSpace(display) ? name : display;
        }
        
        public static T ToEnum<T>(this int? value) where T : struct, Enum
        {
            if (!value.HasValue)
                return default; // trả về giá trị 0 (mặc định) của enum

            return (T)Enum.ToObject(typeof(T), value.Value);
        }
        public static (string module_code, string entity_code) GetEntityCode(this Enum value)
        {
            var moduleCode = value.GetType().Name;
            var entityCode = value.ToString();
            return (moduleCode, entityCode);
        }
        public static int? ToEnumId<TEnum>(this string value) where TEnum : struct, Enum
        {
            // 1. Thử theo tên enum
            if (Enum.TryParse<TEnum>(value, true, out var enumValue))
                return Convert.ToInt32(enumValue);

            // 2. Thử theo DisplayAttribute
            foreach (var field in typeof(TEnum).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                var attr = field.GetCustomAttribute<DisplayAttribute>();
                if (attr != null && attr.Name == value)
                    return (int)field.GetValue(null);
            }
            return null; // Không tìm thấy
        }
        
        public static string? ToDisplayName<TEnum>(this int? value) where TEnum : struct, Enum
            => value.HasValue ? ((TEnum)(object)value.Value).GetDisplayName() : null;
        
        public static string ToDisplayName<TEnum>(this int value) where TEnum : struct, Enum
            => ((TEnum)(object)value).GetDisplayName();
        public static int ToInt(this Enum value) => Convert.ToInt32(value);
        
        public static string GetCode(this Enum p)
            => p.ToString().ToLowerInvariant(); 
        
        public static string GetLowerCode(this Enum p)
            => p.ToString().ToLowerInvariant(); 
        
        public static string ToUpperCode(this Enum value)
            => value.ToString().ToUpperInvariant();
    }
}

