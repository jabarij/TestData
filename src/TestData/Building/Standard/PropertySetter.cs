using System;
using System.Reflection;

namespace TestData.Building.Standard
{
    public class PropertySetter : IPropertySetter
    {
        public PropertySetter() : this(new ReadOnlyAutoPropertyBackingFieldSelector()) { }
        public PropertySetter(IPropertyBackingFieldSelector backingFieldSelector)
        {
            BackingFieldSelector = backingFieldSelector ?? throw new ArgumentNullException(nameof(backingFieldSelector));
        }

        public IPropertyBackingFieldSelector BackingFieldSelector { get; }

        public void SetProperty(object owner, PropertyInfo property, object value)
        {
            if (property.CanWrite)
                property.SetValue(owner, value);
            else
            {
                var backingField = BackingFieldSelector.FindBackingField(property);
                backingField?.SetValue(owner, value);
            }
        }
    }
}