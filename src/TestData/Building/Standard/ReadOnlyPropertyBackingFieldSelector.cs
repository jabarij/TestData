using System;
using System.Reflection;

namespace TestData.Building.Standard
{
    public class ReadOnlyPropertyBackingFieldSelector : IPropertyBackingFieldSelector
    {
        public FieldInfo FindBackingField(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            var backingFieldName = GetBackingFieldName(property.Name);
            return property
                .DeclaringType?
                .GetField(GetBackingFieldName(property.Name), BindingFlags.Instance | BindingFlags.NonPublic);
        }

        protected virtual string GetBackingFieldName(string propertyName) => $"<{propertyName}>k__BackingField";
    }
}