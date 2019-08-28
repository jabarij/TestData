using System;
using System.Linq;
using System.Reflection;

namespace TestData.Building.Standard
{
    public class ConstructorSelector<T> : IConstructorSelector<T>
    {
        public ConstructorSelector(ConstructorSelection constructorSelection = ConstructorSelection.Default)
        {
            ConstructorSelection = constructorSelection;
        }

        public ConstructorSelection ConstructorSelection { get; }

        public ConstructorInfo SelectConstructor()
        {
            var constructorsOrderedByParametersCount = typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .OrderBy(e => e.GetParameters().Length);
            switch (ConstructorSelection)
            {
                case ConstructorSelection.LeastParameters:
                    return constructorsOrderedByParametersCount.First();
                case ConstructorSelection.MostParameters:
                    return constructorsOrderedByParametersCount.Last();
                default:
                    throw Error.Format(new InvalidOperationException($"Enum value {typeof(ConstructorSelection).FullName}.{ConstructorSelection} is not handled."), Errors.EnumValueIsNotHandled);
            }
        }
    }
}