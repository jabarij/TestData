using System;

namespace TestData.Building
{
    public interface ITypedPropertyOverwriter : IPropertyOverwriter
    {
        Type PropertyType { get; }
    }
}