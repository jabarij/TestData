using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestData.Building.Standard
{
    public class InstanceFactory<T> : IInstanceFactory<T>
    {
        public InstanceFactory() : this(new ConstructorSelector<T>()) { }
        public InstanceFactory(ConstructorSelection constructorSelection) : this(new ConstructorSelector<T>(constructorSelection)) { }
        public InstanceFactory(IConstructorSelector<T> constructorChooser)
        {
            ConstructorSelector = constructorChooser ?? throw new ArgumentNullException(nameof(constructorChooser));
        }

        public IConstructorSelector<T> ConstructorSelector { get; }

        public T Create(IEnumerable<INamedPropertyOverwriter> overwriters)
        {
            var targetType = typeof(T);
            T instance;
            instance = CreateInstance(overwriters);
            return instance;
        }

        private T CreateInstance(IEnumerable<INamedPropertyOverwriter> overwriters)
        {
            var constructor = ConstructorSelector.SelectConstructor();
            var parameters =
                from parameter in constructor.GetParameters()
                let overwriter =
                    GetParamOverwriter(parameter, overwriters)
                    ?? PropertyOverwriter.Create(parameter.ParameterType)
                select new
                {
                    Parameter = parameter,
                    Overwriter = overwriter
                };
            return (T)constructor.Invoke(parameters.Select(e => e.Overwriter.GetValue()).ToArray());
        }

        protected virtual IPropertyOverwriter GetParamOverwriter(ParameterInfo parameter, IEnumerable<INamedPropertyOverwriter> overwriters) =>
            overwriters
            .SingleOrDefault(e => string.Equals(e.PropertyName, parameter.Name, StringComparison.Ordinal))
            ?? overwriters
                .FirstOrDefault(e => string.Equals(e.PropertyName, parameter.Name, StringComparison.OrdinalIgnoreCase));
    }
}