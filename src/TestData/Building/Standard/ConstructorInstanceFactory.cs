﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestData.Building.Standard
{
    public class ConstructorInstanceFactory<T> : IInstanceFactory<T>
    {
        public ConstructorInstanceFactory() : this(new ConstructorSelector<T>()) { }
        public ConstructorInstanceFactory(ConstructorSelection constructorSelection) : this(new ConstructorSelector<T>(constructorSelection)) { }
        public ConstructorInstanceFactory(IConstructorSelector<T> constructorSelector)
        {
            ConstructorSelector = Assert.IsNotNull(constructorSelector, nameof(constructorSelector));
        }

        public IConstructorSelector<T> ConstructorSelector { get; }

        public T Create(IEnumerable<INamedPropertyOverwriter> overwriters)
        {
            var constructor = ConstructorSelector.SelectConstructor();
            if (constructor == null)
                Error.Raise(new InvalidOperationException($"Could not select any constructor for type {typeof(T).FullName} using constructor selector of type {ConstructorSelector.GetType().FullName}."),
                    Errors.ConstructorNotFound);

            var parameters = constructor
                .GetParameters()
                .Select(param => GetParameterOverwriter(param, overwriters))
                .Select(overwriter => overwriter.GetValue())
                .ToArray();
            return (T)constructor.Invoke(parameters);
        }

        protected virtual IPropertyOverwriter GetParameterOverwriter(ParameterInfo parameter, IEnumerable<INamedPropertyOverwriter> overwriters) =>
            overwriters.SingleOrDefault(e => string.Equals(e.PropertyName, parameter.Name, StringComparison.Ordinal))
            ?? overwriters.FirstOrDefault(e => string.Equals(e.PropertyName, parameter.Name, StringComparison.OrdinalIgnoreCase))
            ?? (IPropertyOverwriter)new PropertyOverwriter(parameter.ParameterType);
    }
}