using System;
using System.Collections.Generic;

namespace TestData.Building.Standard
{
    public class DelegateInstanceFactory<T> : IInstanceFactory<T>
    {
        private readonly Func<IEnumerable<INamedPropertyOverwriter>, T> _create;
        public DelegateInstanceFactory(Func<T> create) : this(e => create()) { }
        public DelegateInstanceFactory(Func<IEnumerable<INamedPropertyOverwriter>, T> create)
        {
            _create = Assert.IsNotNull(create, nameof(create));
        }

        public T Create(IEnumerable<INamedPropertyOverwriter> overwriters) => _create(overwriters);
    }
}
