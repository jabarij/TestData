using System;

namespace TestData.Building
{
    public class PropertyOverwriter : ITypedPropertyOverwriter
    {
        private readonly object _originalValue;

        public PropertyOverwriter(Type propertyType, object originalValue = null)
        {
            PropertyType = Assert.IsNotNull(propertyType, nameof(propertyType));

            if (!ReferenceEquals(originalValue, null))
                Assert.IsOfType(originalValue, propertyType, nameof(originalValue));

            originalValue = NullCheck(propertyType, originalValue);
            _originalValue = originalValue;
            _value = originalValue;
        }

        public Type PropertyType { get; }

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

        private object NullCheck(Type type, object value) =>
            type.IsValueType && ReferenceEquals(value, null)
            ? Activator.CreateInstance(type)
            : value;
    }
}