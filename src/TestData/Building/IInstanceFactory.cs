using System.Collections.Generic;

namespace TestData.Building
{
    public interface IInstanceFactory<T>
    {
        T Create(IEnumerable<INamedPropertyOverwriter> overwriters);
    }
}