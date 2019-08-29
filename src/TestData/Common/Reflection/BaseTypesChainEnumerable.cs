using System;
using System.Collections;
using System.Collections.Generic;

namespace TestData.Common.Reflection
{
    class BaseTypesChainEnumerable : IEnumerable<Type>
    {
        private readonly Type _type;

        public BaseTypesChainEnumerable(Type type)
        {
            _type = Assert.IsNotNull(type, nameof(type));
        }

        public IEnumerator<Type> GetEnumerator() => new TypeEnumerator(_type, t => t.BaseType);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}