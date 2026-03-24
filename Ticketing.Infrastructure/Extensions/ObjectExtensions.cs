using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Ticketing.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string NonMarkSeo(this string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            string tmp = Regex.Replace(str.ToLower().UnicodetoVn(), "[^a-zA-Z0-9]+", "-");
            tmp = tmp.Replace("---", "--");
            tmp = tmp.Replace("--", "-");
            if (tmp.StartsWith("-"))
                tmp = tmp.Substring(1);
            if (tmp.EndsWith("-"))
                tmp = tmp.Substring(0, tmp.Length - 1);
            return tmp;
        }

        public static string UnicodetoVn(this String str)
        {
            string stFormD = str.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < stFormD.Length; i++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[i]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[i]);
                }
            }

            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }

        public static object[] GetParameterValues(this object obj, string methodName)
        {
            try
            {
                var method = obj.GetType().GetMethod(methodName);
                if (method == null) return Array.Empty<object>();

                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrames()?
                    .FirstOrDefault(f => f.GetMethod()?.DeclaringType == obj.GetType());

                var methodParams = frame?.GetMethod()?.GetParameters();
                if (methodParams == null) return Array.Empty<object>();

                var values = methodParams
                    .Select(param => obj.GetType()
                        .GetFields()
                        .FirstOrDefault(f => f.Name == param.Name)?.GetValue(obj))
                    .ToArray();

                return values;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetParameterValues: {ex.Message}");
                return Array.Empty<object>();
            }
        }

        public static string CutString(this string source, int numchar, int typeStr)
        {
            string text = source;
            try
            {
                string[] array = source.Split(new string[1] { " " }, StringSplitOptions.None);
                if (array.Length > numchar)
                {
                    text = "";
                    for (int i = 0; i <= numchar - 1; i++)
                    {
                        text = text + array[i].ToString() + " ";
                    }

                    text += ((typeStr == 0) ? "..." : "");
                    return text;
                }

                return text;
            }
            catch
            {
                return text;
            }
        }

        public static string StringFormat<T>(this string str, List<T> lst)
        {
            string strResult = string.Empty;

            if (!string.IsNullOrWhiteSpace(str))
            {
                strResult = string.Format(str, lst != null ? string.Join("__", lst) : "__");
            }

            return strResult;
        }

        public static string FormatFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return filePath;
            filePath = filePath.Trim();
            filePath = filePath.Trim(';');
            filePath = filePath.Replace("\\", "/");
            while (filePath.Contains("//"))
            {
                filePath = filePath.Replace("//", "/");
            }

            filePath = filePath.Replace("http:/", "http://");
            filePath = filePath.Replace("https:/", "https://");
            return filePath;
        }

        public static string NumberToText(double inputNumber)
        {
            if (inputNumber == 0) return "không";

            string[] unitNumbers = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = { "", "nghìn", "triệu", "tỷ" };

            bool isNegative = inputNumber < 0;
            long number = (long)Math.Abs(inputNumber); // Bỏ phần thập phân và dấu âm
            string result = "";

            int place = 0;
            while (number > 0)
            {
                int segment = (int)(number % 1000);
                if (segment != 0 || place == 3)
                {
                    string segmentText = ReadThreeDigits(segment, unitNumbers);
                    result = $"{segmentText} {placeValues[place]} {result}".Trim();
                }

                number /= 1000;
                place = (place + 1) % 4;
                if (place == 0 && number > 0) result = "tỷ " + result;
            }

            result = result.Trim();
            if (isNegative) result = "Âm " + result;

            return result;
        }

        public static string Normalize(string? input)
            => input?.Trim().ToLowerInvariant() ?? string.Empty;

        private static string ReadThreeDigits(int number, string[] unitNumbers)
        {
            int hundreds = number / 100;
            int tens = (number % 100) / 10;
            int ones = number % 10;
            string result = "";

            if (hundreds > 0)
                result += unitNumbers[hundreds] + " trăm ";
            else if (tens > 0 || ones > 0)
                result += "không trăm ";

            if (tens > 1)
            {
                result += unitNumbers[tens] + " mươi ";
                if (ones == 1)
                    result += "mốt ";
                else if (ones == 5)
                    result += "lăm ";
                else if (ones > 0)
                    result += unitNumbers[ones] + " ";
            }
            else if (tens == 1)
            {
                result += "mười ";
                if (ones == 5)
                    result += "lăm ";
                else if (ones > 0)
                    result += unitNumbers[ones] + " ";
            }
            else if (ones > 0)
            {
                result += "lẻ " + unitNumbers[ones] + " ";
            }

            return result.Trim();
        }

        public static string GenerateRandom(int intLength)
        {
            StringBuilder strBuilder = new StringBuilder();
            string result = string.Empty;
            string allowedChars = "1,2,3,4,5,6,7,8,9";
            allowedChars += ",A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z";
            allowedChars += ",a,b,c,d,e,f,g,h,i,j,k,m,n,p,q,r,s,t,u,v,w,x,y,z";
            string[] arr = allowedChars.Split(',');
            Random rand = new Random();
            for (int i = 0; i < intLength; i++)
            {
                strBuilder.Append(arr[rand.Next(0, arr.Length)]);
            }

            return strBuilder.ToString();
        }

        public static string ReplaceAll(this string seed, char[] chars, char replacementCharacter)
        {
            return chars.Aggregate(seed, (str, cItem) => str.Replace(cItem, replacementCharacter));
        }

        public static bool ToBool(this string input)
        {
            return int.TryParse(input, out var num) ? num > 0 : bool.TryParse(input, out var flag) && flag;
        }

        public static async Task<bool> ToBoolAsync(this Task<string> task)
        {
            var result = await task.ConfigureAwait(false);
            return result.ToBool();
        }

        public static async Task<bool> ToBoolAsync(this Task<int> task)
        {
            var result = await task.ConfigureAwait(false);
            return result > 0;
        }

        public static int ToInt(this string input)
        {
            if (int.TryParse(input, out int value))
            {
                return value;
            }

            return 0;
        }

        public static async Task<int?> ToIntAsync(this Task<string> task)
        {
            var result = await task.ConfigureAwait(false);
            return result.ToPositiveIntOrNull();
        }

        public static async Task<long?> ToLongAsync(this Task<string> task)
        {
            var result = await task.ConfigureAwait(false);
            return result.ToPositiveLongOrNull();
        }

        public static int? ToPositiveIntOrNull(this string input)
        {
            if (!string.IsNullOrEmpty(input) &&
                int.TryParse(input, out int value) &&
                value > 0)
            {
                return value;
            }

            return null;
        }

        public static long? ToPositiveLongOrNull(this string input)
        {
            if (!string.IsNullOrEmpty(input) &&
                long.TryParse(input, out long value) &&
                value > 0)
            {
                return value;
            }

            return null;
        }
    }

    public static class ObjectExtensions
    {
        public static ConcurrentBag<T> ToConcurrentBag<T>(this IEnumerable<T> source) => new ConcurrentBag<T>(source);


        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                    .GetProperty(item.Key)
                    .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static Dictionary<string, object> AsDictionary(this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
        {
            return source.GetType().GetProperties().ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }

        public static Dictionary<(string name, string type), object> AsDictionaryType(this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => (propInfo.Name, propInfo.PropertyType.ToString()),
                propInfo => propInfo.GetValue(source, null)
            );
        }

        public static double setAverage(params double[] ratios)
        {
            return ratios.Average();
        }

        public static double setDivision(double a, double b, double exp)
        {
            if (b == 0) return exp;
            return Math.Round(a / b, 5);
        }

        public static T ToValue<T>(this object source)
        {
            if (source != null)
            {
                return (T)source;
            }

            return default(T);
        }

        public static Type GetUnderlyingType(this Type t)
            => Nullable.GetUnderlyingType(t) ?? t;


        public static (bool isok, object value) ConvertValueObject(this object value, string datatypeid)
        {
            switch (datatypeid)
            {
                case "int":
                    return (Int32.TryParse(value.ToString(), out var resultint), resultint);
                case "long":
                    return (Int64.TryParse(value.ToString(), out var resultbigint), resultbigint);
                case "double":
                    return (double.TryParse(value.ToString(), out var resultdouble), resultdouble);
                case "bool":
                    return (bool.TryParse(value.ToString(), out bool resultBool), resultBool);
                case "datetime":
                    return (DateTime.TryParse(value.ToString(), out DateTime resultDatetime), resultDatetime);
                case "string":
                    return (true, value.ToString());
                default:
                    return (false, default);
            }
        }

        public static bool IsValueTypeOrString(this Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        public static string ToStringValueType(this object value)
        {
            return value switch
            {
                DateTime dateTime => dateTime.ToString("o"),
                bool boolean => boolean.ToStringLowerCase(),
                _ => value.ToString()
            };
        }

        public static bool HasValue(object value)
        {
            if (value == null) return false;

            return value switch
            {
                bool boolValue => boolValue,
                string strValue => bool.TryParse(strValue, out bool result) && result,
                int intValue => intValue != 0,
                DateTime dtValue => dtValue != DateTime.MinValue,
                double doubleValue => doubleValue != 0,
                IEnumerable<object> listValue => listValue.Any(),
                _ => !string.IsNullOrWhiteSpace(Convert.ToString(value))
            };
        }
        // public static Type GetAttributeType(int datatypeId)
        // {
        //     return (DataTypeEnum)datatypeId switch
        //     {
        //         DataTypeEnum.Integer  => typeof(int),
        //         DataTypeEnum.Double   => typeof(double),
        //         DataTypeEnum.Boolean  => typeof(bool),
        //         DataTypeEnum.DateTime => typeof(DateTime),
        //         _ => typeof(string)
        //     };
        // }

        public static bool IsIEnumerable(this Type type)
        {
            return type.IsAssignableTo(typeof(IEnumerable));
        }

        public static string ToStringLowerCase(this bool boolean)
        {
            return boolean ? "true" : "false";
        }

        public static Type CreateTypeImport<T>()
        {
            // Lấy thông tin Type của T
            var sourceType = typeof(T);

            // Tạo Assembly và Module động
            var assemblyName = new AssemblyName("DynamicAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

            // Tạo TypeBuilder
            var typeBuilder = moduleBuilder.DefineType(sourceType.Name + "_importtype",
                TypeAttributes.Public | TypeAttributes.Class);

            // Lấy các thuộc tính có Attribute [Key]
            var keyProperties = sourceType.GetProperties()
                .Where(prop => prop.GetCustomAttributes(typeof(KeyAttribute), true).Any());

            // Tạo các field tương ứng trong Type động
            foreach (var property in keyProperties)
            {
                typeBuilder.DefineField(property.Name, property.PropertyType, FieldAttributes.Public);
            }

            // Tạo Type
            return typeBuilder.CreateType();
        }


        private static readonly ConcurrentDictionary<Type, PropertyInfo[]>
            FindByPropertiesCache = new();


        private static object ConvertValue<TValue>(TValue value, Type targetType)
            where TValue : struct
        {
            if (targetType == typeof(int)) return Unsafe.As<TValue, int>(ref value);
            if (targetType == typeof(double)) return Unsafe.As<TValue, double>(ref value);
            if (targetType == typeof(float)) return Unsafe.As<TValue, float>(ref value);
            if (targetType == typeof(decimal)) return Unsafe.As<TValue, decimal>(ref value);

            return Convert.ChangeType(value, targetType);
        }

        public static object? ChangeTypeExcel(object value, Type targetType)
        {
            if (value == null || value is DBNull) return null;
            var type = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (type == typeof(string)) return value.ToString();
            if (type == typeof(int)) return int.TryParse(value.ToString(), out var i) ? i : 0;
            if (type == typeof(double)) return double.TryParse(value.ToString(), out var d) ? d : 0.0;
            if (type == typeof(decimal)) return decimal.TryParse(value.ToString(), out var dec) ? dec : 0m;
            if (type == typeof(bool)) return value.ToString() == "1" || value.ToString().ToLower() == "true";
            if (type == typeof(DateTime)) return DateTime.TryParse(value.ToString(), out var dt) ? dt : DateTime.MinValue;
            // Nếu không phải kiểu thường gặp, fallback
            return Convert.ChangeType(value, type);
        }

        public static string GetQueryString(this object obj)
        {
            try
            {
                var properties = from p in obj.GetType().GetProperties()
                    where p.GetValue(obj, null) != null
                    select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());
                return String.Join("&", properties.ToArray());
            }
            catch
            {
            }

            return "";
        }

        public static string GetQueryString(this object[] args)
        {
            try
            {
                var properties = from p in args
                    where p != null
                    select p + "=" + HttpUtility.UrlEncode(p.ToString());
                return String.Join("&", properties.ToArray());
            }
            catch
            {
            }

            return "";
        }
    }
}