using System.Reflection;

namespace TestData.Building
{
    internal class PropertyInfoOverwriter : NamedPropertyOverwriter, INamedPropertyOverwriter
    {
        public PropertyInfoOverwriter(PropertyInfo property, object originalValue = null)
            : base(Assert.IsNotNull(property, nameof(property)).Name, property.PropertyType, originalValue)
        {
            Property = property;
        }

        public PropertyInfo Property { get; }

        public void SetValueFromPropertyOwner(object owner) =>
            SetValue(Property.GetValue(owner));
    }
}