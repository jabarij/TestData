using System;

namespace TestData.Building
{
    public class NamedPropertyOverwriter : PropertyOverwriter, INamedPropertyOverwriter
    {
        public NamedPropertyOverwriter(string propertyName, Type propertyType, object originalValue = null)
            : base(propertyType, originalValue)
        {
            PropertyName = Assert.IsNotNullOrWhiteSpace(propertyName, nameof(propertyName));
        }

        public string PropertyName { get; }
    }

    public class NamedPropertyOverwriter<T> : NamedPropertyOverwriter
    {
        public NamedPropertyOverwriter(string propertyName, T originalValue = default(T))
            : base(propertyName, typeof(T), originalValue) { }
    }
}