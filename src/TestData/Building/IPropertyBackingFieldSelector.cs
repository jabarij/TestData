using System.Collections.Generic;
using System.Reflection;

namespace TestData.Building
{
    public interface IPropertyBackingFieldSelector
    {
        FieldInfo FindBackingField(PropertyInfo property);
    }
}