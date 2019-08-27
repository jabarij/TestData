using System;
using System.Collections.Generic;
using System.Reflection;

namespace TestData.Building.Dynamic
{
    public interface IMemberSelector
    {
        IEnumerable<PropertyInfo> SelectProperties(Type type);
    }
}