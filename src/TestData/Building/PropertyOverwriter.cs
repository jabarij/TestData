using System;

namespace TestData.Building
{
    public class PropertyOverwriter : IPropertyOverwriter
    {
        private readonly Type _propertyType;
        private readonly object _originalValue;

        public PropertyOverwriter(Type propertyType, object originalValue = null)
        {
            _propertyType = Assert.IsNotNull(propertyType, nameof(propertyType));

            if (!ReferenceEquals(originalValue, null))
                Assert.IsOfType(originalValue, propertyType, nameof(originalValue));

            originalValue = NullCheck(propertyType, originalValue);
            _originalValue = originalValue;
            _value = originalValue;
        }

        private object NullCheck(Type type, object value) =>
            type.IsValueType && ReferenceEquals(value, null)
            ? Activator.CreateInstance(type)
            : value;

        public bool IsOverwritten { get; private set; }
        public void Restore()
        {
            _value = _originalValue;
            IsOverwritten = true;
        }

        private object _value;
        public object Value { get => _value; set { _value = value; IsOverwritten = true; } }

        public object GetValue() => Value;
        public void SetValue(object value) => Value = value;
    }
}