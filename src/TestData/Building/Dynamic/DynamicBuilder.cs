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
        private readonly IReadOnlyCollection<PropertyInfo> _properties;
        private readonly HashSet<INamedPropertyOverwriter> _propertyOverwriters =
            new HashSet<INamedPropertyOverwriter>(
                new DelegatedEqualityComparer<INamedPropertyOverwriter>(
                    (a, b) => string.Equals(a.PropertyName, b.PropertyName, StringComparison.Ordinal),
                    obj => obj.PropertyName.GetHashCode()));

        public DynamicBuilder(
            IInstanceFactory<T> instanceFactory = null,
            IPropertySetter propertySetter = null,
            IMemberSelector memberSelector = null)
        {
            InstanceFactory = instanceFactory ?? new ConstructorInstanceFactory<T>();
            PropertySetter = propertySetter ?? new PropertySetter();
            MemberSelector = memberSelector ?? new MemberSelector();
            _properties = new ReadOnlyCollection<PropertyInfo>(MemberSelector.SelectProperties(typeof(T)).ToList());
        }

        public IInstanceFactory<T> InstanceFactory { get; }
        public IPropertySetter PropertySetter { get; }
        public IMemberSelector MemberSelector { get; }

        public void Overwrite(string name, object value)
        {
            Assert.IsNotNullOrWhiteSpace(name, nameof(name));
            var property = GetPropertyOrThrow(name);
            Overwrite(new PropertyInfoOverwriter(property, value));
        }
        public void OverwriteWithDelegate(Action<OverwritingContext> overwrite)
        {
            Assert.IsNotNull(overwrite, nameof(overwrite));

            var overwritingContexts = _properties
                .Where(e => e.CanRead)
                .Select(e => new OverwritingContext(e.Name, e.PropertyType));
            foreach (var overwritingContext in overwritingContexts)
            {
                overwrite(overwritingContext);
                if (overwritingContext.IsOverwritten)
                    Overwrite(overwritingContext);
            }
        }
        public void OverwriteWithTemplate(object template)
        {
            Assert.IsNotNull(template, nameof(template));

            if (template is T)
            {
                var propertyOverwriters = _properties
                    .Where(e => e.CanRead)
                    .Select(e => new PropertyInfoOverwriter(e));
                foreach (var overwriter in propertyOverwriters)
                {
                    overwriter.SetValueFromPropertyOwner(template);
                    Overwrite(overwriter);
                }
            }
            else
            {
                var theirProperties = MemberSelector.SelectProperties(template.GetType()).Where(e => e.CanRead).ToList();
                var propertyMatches = _properties
                    .GroupJoin(
                        inner: theirProperties,
                        outerKeySelector: ourP => ourP,
                        innerKeySelector: theirP => theirP,
                        resultSelector: (ourP, theirP) => new { OurProperty = ourP, TheirProperties = theirP },
                        comparer: new DelegatedEqualityComparer<PropertyInfo>(MatchProperties, p => p.Name.ToLower().GetHashCode()));
                var entries =
                    from match in propertyMatches
                    let propertiesComparer = new DelegatedComparer<PropertyInfo>(CompareProperties(match.OurProperty))
                    select new
                    {
                        Overwriter = new PropertyInfoOverwriter(match.OurProperty),
                        TheirProperty = match.TheirProperties
                            .OrderBy(p => p, propertiesComparer)
                            .FirstOrDefault()
                    };
                foreach (var entry in entries)
                {
                    if (entry.TheirProperty == null)
                        continue;
                    var overwriter = entry.Overwriter;
                    overwriter.SetValue(entry.TheirProperty.GetValue(template));
                    Overwrite(overwriter);
                }
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
            Assert.IsNotNullOrWhiteSpace(name, nameof(name));

            var property = GetPropertyOrThrow(name);
            return _propertyOverwriters.Any(e => string.Equals(e.PropertyName, property.Name, StringComparison.Ordinal));
        }
        public object GetOverwrittenValue(string name)
        {
            Assert.IsNotNullOrWhiteSpace(name, nameof(name));
            var property = GetPropertyOrThrow(name);
            var overwriter = new PropertyInfoOverwriter(property);
            return
                _propertyOverwriters.TryGetValue(overwriter, out INamedPropertyOverwriter existingOverwriter)
                ? existingOverwriter.GetValue()
                : overwriter.Value;
        }

        public sealed override T Build()
        {
            T instance = InstanceFactory.Create(_propertyOverwriters);

            var overwrittenProperties =
                from property in _properties
                join overwriter in _propertyOverwriters on property.Name equals overwriter.PropertyName
                select (Property: property, Overwriter: overwriter);
            foreach (var entry in overwrittenProperties)
                PropertySetter.SetProperty(instance, entry.Property, entry.Overwriter.GetValue());

            return instance;
        }

        private PropertyInfo GetPropertyOrThrow(string name)
        {
            var property =
                _properties.SingleOrDefault(e => string.Equals(e.Name, name, StringComparison.Ordinal))
                ?? _properties.FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));
            if (property == null)
                throw new InvalidOperationException($"Unknown property '{name}' of type {typeof(T).FullName}.");
            return property;
        }
        private bool MatchProperties(PropertyInfo source, PropertyInfo target) =>
            string.Equals(source.Name, target.Name, StringComparison.OrdinalIgnoreCase)
            && target.PropertyType.IsAssignableFrom(source.PropertyType);
        private Func<PropertyInfo, PropertyInfo, int> CompareProperties(PropertyInfo sourceProperty) =>
            ((p1, p2) =>
            {
                if (string.Equals(p1.Name, p2.Name, StringComparison.Ordinal))
                    return 0;
                if (string.Equals(sourceProperty.Name, p1.Name, StringComparison.Ordinal))
                    return 1;
                if (string.Equals(sourceProperty.Name, p2.Name, StringComparison.Ordinal))
                    return -1;
                if (string.Equals(sourceProperty.Name, p1.Name, StringComparison.OrdinalIgnoreCase))
                    return 1;
                if (string.Equals(sourceProperty.Name, p2.Name, StringComparison.OrdinalIgnoreCase))
                    return -1;
                return string.CompareOrdinal(p1.Name, p2.Name);
            });
    }
}
