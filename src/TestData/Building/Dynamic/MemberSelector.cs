using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestData.Building.Dynamic
{
    public class MemberSelector : IMemberSelector
    {
        public IEnumerable<PropertyInfo> SelectProperties(Type type) =>
            from property in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            let getMethod = property.GetGetMethod(true)
            where
               getMethod.IsPublic
               || getMethod.IsAssembly
               || getMethod.IsFamilyOrAssembly
            select property;
    }
}