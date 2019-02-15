using System;
using System.Collections.Generic;

namespace TestData.Building.Standard
{
    public class FixedInstanceFactory<T> : IInstanceFactory<T>
        where T : class
    {
        public FixedInstanceFactory(T instance)
        {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public T Instance { get; }

        public T Create(IEnumerable<INamedPropertyOverwriter> overwriters) => Instance;
    }
}
