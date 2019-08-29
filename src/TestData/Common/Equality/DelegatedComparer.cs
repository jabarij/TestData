using System;
using System.Collections.Generic;

namespace TestData.Common.Equality
{
    class DelegatedComparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _compare;

        public DelegatedComparer(Func<T, T, int> compare)
        {
            _compare = Assert.IsNotNull(compare, nameof(compare));
        }

        public int Compare(T x, T y) =>
            _compare(x, y);
    }
}
