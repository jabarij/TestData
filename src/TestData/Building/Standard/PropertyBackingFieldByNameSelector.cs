using System;
using System.Reflection;

namespace TestData.Building.Standard
{
    public abstract class PropertyBackingFieldByNameSelector : IPropertyBackingFieldSelector
    {
        public FieldInfo FindBackingField(PropertyInfo property)
        {
            Assert.IsNotNull(property, nameof(property));

            var backingFieldName = GetBackingFieldName(property);
            if (string.IsNullOrWhiteSpace(backingFieldName))
                Error.Raise(new InvalidOperationException("Backing field name must not be either null, empty string or consist of white spaces only."), Errors.ObjectIsNullOrWhiteSpace);
            return property
                .DeclaringType?
                .GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        protected abstract string GetBackingFieldName(PropertyInfo property);
    }
}
