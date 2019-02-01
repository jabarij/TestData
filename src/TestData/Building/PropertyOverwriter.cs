using System;

namespace TestData.Building
{
    public class PropertyOverwriter<T> : IPropertyOverwriter<T>
    {
        private readonly T _originalValue;

        private PropertyOverwriter()
            : this(default(T)) { }
        public PropertyOverwriter(T originalValue)
        {
            _originalValue = originalValue;
            _value = originalValue;
        }

        public bool IsOverwritten { get; private set; }
        public void Restore()
        {
            _value = _originalValue;
            IsOverwritten = true;
        }

        private T _value;
        public T Value { get => _value; set { _value = value; IsOverwritten = true; } }

        object IValueGetter.GetValue() => Value;
        void IValueSetter.SetValue(object value) => Value = (T)value;

    }

    public class PropertyOverwriter
    {
        public static IPropertyOverwriter Create(Type type) =>
            (IPropertyOverwriter)Activator.CreateInstance(typeof(PropertyOverwriter<>).MakeGenericType(type), true);
        public static PropertyOverwriter<T> Create<T>() =>
            new PropertyOverwriter<T>(default(T));
        public static PropertyOverwriter<T> Create<T>(T originalValue) =>
            new PropertyOverwriter<T>(originalValue);
    }
}