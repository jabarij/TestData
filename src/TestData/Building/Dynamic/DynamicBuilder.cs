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
        private static readonly IReadOnlyCollection<PropertyInfo> Properties;
        static DynamicBuilder()
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .ToList();
            Properties = new ReadOnlyCollection<PropertyInfo>(properties);
        }

        private readonly HashSet<INamedPropertyOverwriter> _propertyOverwriters =
            new HashSet<INamedPropertyOverwriter>(
                new DelegatedEqualityComparer<INamedPropertyOverwriter>(
                    (a, b) => ArePropertyNamesEqual(a.PropertyName, b.PropertyName),
                    obj => obj.PropertyName.GetHashCode()));
        private static bool ArePropertyNamesEqual(string propertyName1, string propertyName2) =>
            string.Equals(propertyName1, propertyName2, StringComparison.Ordinal);

        public DynamicBuilder() : this(new InstanceFactory<T>()) { }
        public DynamicBuilder(ConstructorSelection constructorSelection) : this(StandardBuild.CreateInstanceFactory<T>(constructorSelection)) { }
        public DynamicBuilder(IInstanceFactory<T> instanceFactory) : this(instanceFactory, new ReadOnlyAutoPropertyBackingFieldSelector()) { }
        public DynamicBuilder(IPropertyBackingFieldSelector propertyBackingFieldSelector) : this(StandardBuild.CreateInstanceFactory<T>(), propertyBackingFieldSelector) { }
        public DynamicBuilder(IInstanceFactory<T> instanceFactory, IPropertyBackingFieldSelector propertyBackingFieldSelector)
        {
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
            PropertyBackingFieldSelector = propertyBackingFieldSelector ?? throw new ArgumentNullException(nameof(propertyBackingFieldSelector));
        }

        public IInstanceFactory<T> InstanceFactory { get; }
        public IPropertyBackingFieldSelector PropertyBackingFieldSelector { get; }

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
            return _propertyOverwriters.Any(e => ArePropertyNamesEqual(e.PropertyName, property.Name));
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
            OverwriteReadOnlyProperties(instance);
            OverwriteProperties(instance);
            return instance;
        }

        private void OverwriteReadOnlyProperties(T instance)
        {
            var overwrittenProperties =
                from property in Properties
                where !property.CanWrite
                join overwriter in _propertyOverwriters on property.Name equals overwriter.PropertyName
                select new
                {
                    Property = property,
                    Overwriter = overwriter
                };
            foreach (var property in overwrittenProperties)
            {
                var backingField = GetBackingFieldOrThrow(property.Property);
                backingField.SetValue(instance, property.Overwriter.GetValue());
            }
        }

        private void OverwriteProperties(T instance)
        {
            var overwrittenProperties =
                from property in Properties
                where property.CanWrite
                join overwriter in _propertyOverwriters on property.Name equals overwriter.PropertyName
                select new
                {
                    Property = property,
                    Overwriter = overwriter
                };
            foreach (var property in overwrittenProperties)
                property.Property.SetValue(instance, property.Overwriter.GetValue());
        }

        private FieldInfo GetBackingFieldOrThrow(PropertyInfo property) =>
            PropertyBackingFieldSelector.FindBackingField(property)
            ?? throw new InvalidOperationException($"Could not find backing field for property {property.DeclaringType.FullName}.{property.Name} using selector of type {PropertyBackingFieldSelector.GetType().FullName}.");
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
