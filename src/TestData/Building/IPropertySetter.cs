using System.Reflection;

namespace TestData.Building
{
    public interface IPropertySetter
    {
        void SetProperty(object owner, PropertyInfo property, object value);
    }
}