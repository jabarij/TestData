using System;
using System.Collections.Generic;

namespace TestData.Common.Equality
{
    class DelegatedEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _equals;
        private readonly Func<T, int> _getHashCode;

        public DelegatedEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode = null)
        {
            _equals = Assert.IsNotNull(equals, nameof(equals));
            _getHashCode = getHashCode ?? (e => e.GetHashCode());
        }

        public bool Equals(T x, T y) => _equals(x, y);
        public int GetHashCode(T obj) => _getHashCode(obj);
    }
}
