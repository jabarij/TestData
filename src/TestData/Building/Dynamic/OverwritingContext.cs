using System;

namespace TestData.Building.Dynamic
{
    public class OverwritingContext : NamedPropertyOverwriter
    {
        internal OverwritingContext(string propertyName, Type propertyType)
            : base(propertyName, propertyType, null) { }
    }
}