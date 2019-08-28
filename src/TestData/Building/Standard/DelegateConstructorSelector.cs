using System;
using System.Reflection;

namespace TestData.Building.Standard
{
    public class DelegateConstructorSelector<T> : IConstructorSelector<T>
    {
        public DelegateConstructorSelector(Func<Type, ConstructorInfo> selectConstructor)
        {
            _selectConstructor = Assert.IsNotNull(selectConstructor, nameof(selectConstructor));
        }

        private readonly Func<Type, ConstructorInfo> _selectConstructor;

        public ConstructorInfo SelectConstructor() => _selectConstructor(typeof(T));
    }
}