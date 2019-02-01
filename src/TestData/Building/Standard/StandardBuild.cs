using System;
using System.Collections.Generic;

namespace TestData.Building.Standard
{
    public static class StandardBuild
    {
        public static DelegateInstanceFactory<T> CreateInstanceFactoryFromDelegate<T>(Func<T> create) =>
            new DelegateInstanceFactory<T>(e => create());
        public static DelegateInstanceFactory<T> CreateInstanceFactoryFromDelegate<T>(Func<IEnumerable<INamedPropertyOverwriter>, T> create) =>
            new DelegateInstanceFactory<T>(create);

        public static InstanceFactory<T> CreateInstanceFactory<T>(ConstructorSelection constructorSelection) =>
            new InstanceFactory<T>(constructorSelection);
        public static InstanceFactory<T> CreateInstanceFactory<T>() =>
            new InstanceFactory<T>();
    }
}
