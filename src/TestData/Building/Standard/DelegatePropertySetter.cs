using System;
using System.Reflection;

namespace TestData.Building.Standard
{
    public class DelegatePropertySetter : IPropertySetter
    {
        private readonly Action<object, PropertyInfo, object> _setProperty;

        public DelegatePropertySetter(Action<object, PropertyInfo, object> setProperty)
        {
            _setProperty = Assert.IsNotNull(setProperty, nameof(setProperty));
        }

        public void SetProperty(object owner, PropertyInfo property, object value) => _setProperty(owner, property, value);
    }
}