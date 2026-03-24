namespace Ticketing.Infrastructure.Extensions
{
    public static class ValueObjectExtensions
    {
        public static void ReplaceValueObject<T>(this IList<T> list, T newValue)
        where T : class
        {
            if (newValue == null)
                return;

            // Lấy tất cả property chung của ValueObject
            var entityIdProp = typeof(T).GetProperty("EntityId");
            var attributeIdProp = typeof(T).GetProperty("AttributeId");

            if (entityIdProp == null || attributeIdProp == null)
                throw new InvalidOperationException("ValueObject must contain EntityId and AttributeId");

            var newEntityId = entityIdProp.GetValue(newValue);
            var newAttributeId = attributeIdProp.GetValue(newValue);

            // Xoá record cũ
            var old = list
                .FirstOrDefault(x =>
                                    entityIdProp.GetValue(x)!.Equals(newEntityId)
                                    && attributeIdProp.GetValue(x)!.Equals(newAttributeId)
                );

            if (old != null)
                list.Remove(old);

            // Thêm record mới
            list.Add(newValue);
        }
    }
}
