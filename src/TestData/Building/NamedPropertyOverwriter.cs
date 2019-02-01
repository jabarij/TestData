using System;
using System.Reflection;

namespace TestData.Building
{
    public class NamedPropertyOverwriter<T> : PropertyOverwriter<T>, INamedPropertyOverwriter
    {
        public NamedPropertyOverwriter(string propertyName) : this(propertyName, default(T)) { }
        public NamedPropertyOverwriter(string propertyName, T originalValue)
            : base(originalValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));
            PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }

    public static class NamedPropertyOverwriter
    {
        public static INamedPropertyOverwriter Create(PropertyInfo property) =>
            (INamedPropertyOverwriter)Activator.CreateInstance(typeof(NamedPropertyOverwriter<>).MakeGenericType(property.PropertyType), property.Name);
        public static INamedPropertyOverwriter Create(Type type, string propertyName) =>
            (INamedPropertyOverwriter)Activator.CreateInstance(typeof(NamedPropertyOverwriter<>).MakeGenericType(type), propertyName);
    }
}