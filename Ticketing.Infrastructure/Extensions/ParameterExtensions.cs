using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.ObjectPool;
using Ticketing.Infrastructure.Attributes;

namespace Ticketing.Infrastructure.Extensions;

public static class ParameterExtensions
{
    private static readonly ConcurrentDictionary<Type, PropertyDescriptor> PropertyCache = new();
    
    private static readonly ObjectPool<List<object>> ParameterListPool = 
        new DefaultObjectPool<List<object>>(new ParameterListPooledObjectPolicy(), maximumRetained: 50);

    private readonly record struct PropertyDescriptor(PropertyInfo[] Properties, string[] Names);

    private static PropertyDescriptor GetStableProperties(Type type)
    {
        return PropertyCache.GetOrAdd(type, t => 
        {
            var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetMethod is { IsPublic: true } && p.GetIndexParameters().Length == 0) // Chỉ lấy property có public getter thật sự
                .OrderBy(p => p.Name, StringComparer.Ordinal) // Cố định thứ tự theo Ordinal để tránh culture differences
                .ToArray();
            
            // Cache luôn property names để tránh đọc prop.Name lại
            var names = properties.Select(p => p.Name).ToArray();
            
            return new PropertyDescriptor(properties, names);
        });
    }
    
    
    
    public static object[] ToParameterArrayOld(this object obj)
    {
        if (obj == null)
            return Array.Empty<object>();

        var list = new List<object>();
        if (obj is IDictionary<string, object> dict)
        {
            foreach (var kvp in dict)
            {
                list.Add(kvp.Key);
                list.Add(kvp.Value ?? DBNull.Value);
            }
        }
        else
        {
            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(obj, null) ?? DBNull.Value;

                list.Add(name);
                list.Add(value);
            }
        }

        return list.ToArray();
    }
  
    public static object[] ToParameterArray(this object obj)
    {
        if (obj == null) return [];
        if (obj is IDictionary<string, object> dict)
        {
            var list = ParameterListPool.Get();
            try
            {
                list.Clear();
                foreach (var kv in dict)
                {
                    list.Add(kv.Key);
                    list.Add(kv.Value ?? DBNull.Value);
                }
                return list.ToArray();
            }
            finally { ParameterListPool.Return(list); }
        }
        var type = obj.GetType();
        var descriptor = GetStableProperties(type); // Dùng shared logic với cached names
        

        // Sử dụng object pool để tránh allocations
        var paramList = ParameterListPool.Get();
        
        try
        {
            paramList.Clear();
            
            // Luôn thêm cả property để đồng nhất với compiled mapper
            for (int i = 0; i < descriptor.Properties.Length; i++)
            {
                
                if (Attribute.IsDefined(descriptor.Properties[i], typeof(DbParamIgnoreAttribute)))
                    continue;
                
                var prop = descriptor.Properties[i];
                var propName = descriptor.Names[i]; // Dùng cached name thay vì prop.Name
                
                var value = prop.GetValue(obj);
                if (value is Enum) 
                    value = Convert.ToInt32(value);
                paramList.Add(propName);
                
                // Xử lý null đúng cách: chỉ convert thành DBNull.Value nếu là reference type hoặc nullable
                if (value == null && (prop.PropertyType.IsClass || Nullable.GetUnderlyingType(prop.PropertyType) != null))
                {
                    paramList.Add(DBNull.Value); // Convert null to DBNull for ADO.NET compatibility
                }
                else
                {
                    paramList.Add(value ?? DBNull.Value); // Fallback, though value types shouldn't be null here
                }
            }

            return paramList.ToArray();
        }
        finally
        {
            ParameterListPool.Return(paramList);
        }
    }
    public static class PrecomputedParameters
    {
        // Cache theo Type thay vì string để tránh string allocation và collision
        private static readonly ConcurrentDictionary<Type, Func<object, object[]>> ParameterMappers = new();

        // Memory monitoring
        public static int CachedTypes => ParameterMappers.Count;
        public static long EstimatedMemoryUsage => ParameterMappers.Count * 1500; // ~1.5KB per mapper

        public static object[] GetParameters<T>(T request) where T : class
        {
            // Gọi sang non-generic overload để tránh duplicate logic
            return GetParameters(request);
        }

        /// <summary>
        /// Non-generic version dùng runtime type - đơn giản và an toàn với inheritance
        /// </summary>
        public static object[] GetParameters(object? request)
        {
            if (request == null) return Array.Empty<object>();
            
            var runtimeType = request.GetType();
            var mapper = ParameterMappers.GetOrAdd(runtimeType, type => CreateCompiledMapper(type));
            return mapper(request);
        }

        /// <summary>
        /// Tạo compiled mapper using Expression trees để tối ưu performance
        /// </summary>
        private static Func<object, object[]> CreateCompiledMapper(Type type)
        {
            // Dùng shared logic để đồng bộ với ToOptimizedParameterArray
            var descriptor = GetStableProperties(type);

            // Compile expression-based getter
            var parameter = Expression.Parameter(typeof(object), "obj");
            var typedParameter = Expression.Convert(parameter, type);
            
            var expressions = new List<Expression>();
            var resultVariable = Expression.Variable(typeof(object[]), "result");
            
            // Create result array with exact size (property count * 2)
            var arraySize = Expression.Constant(descriptor.Properties.Length * 2);
            var newArray = Expression.NewArrayBounds(typeof(object), arraySize);
            var assignArray = Expression.Assign(resultVariable, newArray);
            expressions.Add(assignArray);

            // Add each property name and value to array
            for (int i = 0; i < descriptor.Properties.Length; i++)
            {
                var prop = descriptor.Properties[i];
                var propName = descriptor.Names[i]; // Dùng cached name
                var propAccess = Expression.Property(typedParameter, prop);
                
                Expression propValue;
                
                // Fix: Xử lý đúng value types vs reference types
                if (prop.PropertyType.IsValueType && Nullable.GetUnderlyingType(prop.PropertyType) == null)
                {
                    // Non-nullable value type: không thể null, chỉ box thẳng
                    propValue = Expression.Convert(propAccess, typeof(object));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                {
                    // Nullable<T>: tối ưu vi mô với HasValue check
                    var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType)!;
                    var hasValueProperty = prop.PropertyType.GetProperty("HasValue")!;
                    var valueProperty = prop.PropertyType.GetProperty("Value")!;
                    
                    var hasValueAccess = Expression.Property(propAccess, hasValueProperty);
                    var valueAccess = Expression.Property(propAccess, valueProperty);
                    
                    propValue = Expression.Condition(
                        hasValueAccess,
                        Expression.Convert(valueAccess, typeof(object)),
                        Expression.Convert(Expression.Constant(DBNull.Value), typeof(object))
                    );
                }
                else
                {
                    // Reference type: null check thông thường
                    propValue = Expression.Condition(
                        Expression.Equal(propAccess, Expression.Constant(null, prop.PropertyType)),
                        Expression.Convert(Expression.Constant(DBNull.Value), typeof(object)),
                        Expression.Convert(propAccess, typeof(object))
                    );
                }
                
                // Add property name - dùng cached name thay vì Expression.Constant(prop.Name)
                var nameIndex = Expression.Constant(i * 2);
                var nameValue = Expression.Constant(propName); // Cached name
                var setName = Expression.Assign(
                    Expression.ArrayAccess(resultVariable, nameIndex), 
                    nameValue);
                expressions.Add(setName);
                
                // Add property value
                var valueIndex = Expression.Constant(i * 2 + 1);
                var setValue = Expression.Assign(
                    Expression.ArrayAccess(resultVariable, valueIndex), 
                    propValue);
                expressions.Add(setValue);
            }

            // Return the array
            expressions.Add(resultVariable);
            
            var block = Expression.Block(new[] { resultVariable }, expressions);
            var lambda = Expression.Lambda<Func<object, object[]>>(block, parameter);
            
            return lambda.Compile();
        }

        /// <summary>
        /// Clear cache nếu cần - bỏ GC.Collect() để tránh performance impact
        /// </summary>
        public static void ClearCacheIfNeeded(long maxMemoryBytes = 1024 * 1024) // 1MB default
        {
            if (EstimatedMemoryUsage > maxMemoryBytes)
            {
                ParameterMappers.Clear();
                // Bỏ GC.Collect() - let GC handle naturally
            }
        }
    }
}

public class ParameterListPooledObjectPolicy : IPooledObjectPolicy<List<object>>
{
    private const int MaxCapacity = 256; 
    private const int InitialCapacity = 16; 

    public List<object> Create() => new List<object>(InitialCapacity);

    public bool Return(List<object> obj)
    {
        if (obj == null) return false;
        
        // Không pool list quá to để tránh memory leak
        if (obj.Capacity > MaxCapacity) return false;
        
        obj.Clear();
        return true;
    }
}
