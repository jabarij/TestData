using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestData.Common.Collections;
using TestData.Common.Reflection;

namespace TestData.Building
{
    public abstract class Builder<T> : IOverwritableBuilder<T>
    {
        protected Builder()
        {
            InitReadonlyProperties();
        }

        private void InitReadonlyProperties()
        {
            var fields = GetType().GetFields(BindingFlags.SetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldsToInit =
                from field in fields
                where
                    field.IsInitOnly
                    && typeof(IPropertyOverwriter).IsAssignableFrom(field.FieldType)
                    && field.FieldType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[0], null) != null
                select field;
            foreach (var field in fieldsToInit)
            {
                var value = Activator.CreateInstance(field.FieldType, true);
                field.SetValue(this, value);
            }
        }

        public abstract T Build();
        object IBuilder.Build() => Build();

        public virtual bool IsOverwritten() =>
            GetPropertyOverwriters().Any(e => e.IsOverwritten)
            || GetPropertyBuilders().OfType<IOverwritableBuilder>().Any(e => e.IsOverwritten());
        protected virtual IEnumerable<IPropertyOverwriter> GetPropertyOverwriters() =>
            GetType()
            .GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
            .Where(e => typeof(IPropertyOverwriter).IsAssignableFrom(e.PropertyType))
            .Select(e => (IPropertyOverwriter)e.GetValue(this))
            .WhereNotNull();
        protected virtual IEnumerable<IBuilder> GetPropertyBuilders() =>
            GetType()
            .GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
            .Where(e => e.PropertyType.ImplementsInterface(typeof(IBuilder)))
            .Select(e => (IBuilder)e.GetValue(this))
            .WhereNotNull();
    }
}