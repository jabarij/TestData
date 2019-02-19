using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TestData.Building.Standard;
using TestData.Common.Equality;

namespace TestData.Building.Dynamic
{
    public sealed class DynamicBuilder<T> : Builder<T>, IDynamicBuilder<T>
    {
        private static readonly IReadOnlyCollection<PropertyInfo> Properties =
            new ReadOnlyCollection<PropertyInfo>(
                (from property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                 let getMethod = property.GetGetMethod(true)
                 where
                    getMethod.IsPublic
                    || getMethod.IsAssembly
                    || getMethod.IsFamilyOrAssembly
                 select property).ToList());

        private readonly HashSet<INamedPropertyOverwriter> _propertyOverwriters =
            new HashSet<INamedPropertyOverwriter>(
                new DelegatedEqualityComparer<INamedPropertyOverwriter>(
                    (a, b) => string.Equals(a.PropertyName, b.PropertyName, StringComparison.Ordinal),
                    obj => obj.PropertyName.GetHashCode()));

        public DynamicBuilder() : this(CreateDefaultInstanceFactory(), CreateDefaultPropertySetter()) { }
        public DynamicBuilder(IInstanceFactory<T> instanceFactory) : this(instanceFactory, CreateDefaultPropertySetter()) { }
        public DynamicBuilder(IPropertySetter propertySetter) : this(CreateDefaultInstanceFactory(), propertySetter) { }
        public DynamicBuilder(IInstanceFactory<T> instanceFactory, IPropertySetter propertySetter)
        {
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
            PropertySetter = propertySetter ?? throw new ArgumentNullException(nameof(propertySetter));
        }

        public static IInstanceFactory<T> CreateDefaultInstanceFactory() => new ConstructorInstanceFactory<T>();
        public static IPropertySetter CreateDefaultPropertySetter() => new PropertySetter();

        public IInstanceFactory<T> InstanceFactory { get; }
        public IPropertySetter PropertySetter { get; }

        public void Overwrite<TProperty>(string name, TProperty value)
        {
            var property = GetPropertyOrThrow(name);
            Overwrite(new NamedPropertyOverwriter<TProperty>(property.Name, value));
        }
        public void OverwriteAll(T template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            var readableProperties = Properties.Where(e => e.CanRead);
            foreach (var property in readableProperties)
            {
                var overwriter = NamedPropertyOverwriter.Create(property);
                overwriter.SetValue(property.GetValue(template));
                Overwrite(overwriter);
            }
        }
        private void Overwrite(INamedPropertyOverwriter overwriter)
        {
            if (!_propertyOverwriters.TryGetValue(overwriter, out INamedPropertyOverwriter existingOverwriter))
                _propertyOverwriters.Add(overwriter);
            else
                existingOverwriter.SetValue(overwriter.GetValue());
        }
        public bool IsOverwritten(string name)
        {
            var property = GetPropertyOrThrow(name);
            return _propertyOverwriters.Any(e => string.Equals(e.PropertyName, property.Name, StringComparison.Ordinal));
        }
        public TProperty GetOverwrittenValue<TProperty>(string name)
        {
            var property = GetPropertyOrThrow(name);
            var overwriter = new NamedPropertyOverwriter<TProperty>(property.Name);
            return
                _propertyOverwriters.TryGetValue(overwriter, out INamedPropertyOverwriter existingOverwriter)
                ? (TProperty)existingOverwriter.GetValue()
                : overwriter.Value;
        }

        public sealed override T Build()
        {
            T instance = InstanceFactory.Create(_propertyOverwriters);

            var overwrittenProperties =
                from property in Properties
                join overwriter in _propertyOverwriters on property.Name equals overwriter.PropertyName
                select (Property: property, Overwriter: overwriter);
            foreach (var entry in overwrittenProperties)
                PropertySetter.SetProperty(instance, entry.Property, entry.Overwriter.GetValue());

            return instance;
        }

        private static PropertyInfo GetPropertyOrThrow(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            var property =
                Properties.SingleOrDefault(e => string.Equals(e.Name, name, StringComparison.Ordinal))
                ?? Properties.FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));
            if (property == null)
                throw new InvalidOperationException($"Unknown property '{name}' of type {typeof(T).FullName}.");
            return property;
        }
    }
}
