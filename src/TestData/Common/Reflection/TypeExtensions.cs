using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TestData.Common.Reflection
{
    public static class TypeExtensions
    {
        public static bool ImplementsInterface(this Type type, Type @interface) =>
            ImplementsInterface(type, i => i == @interface || @interface.IsGenericTypeDefinition && i.IsGenericType && i.GetGenericTypeDefinition() == @interface);
        public static bool ImplementsInterface(this Type type, Predicate<Type> interfacePredicate) =>
            type
            .FindInterfaces(interfacePredicate)
            .Any();

        public static IEnumerable<Type> FindInterfaces(this Type type, Type @interface) =>
            FindInterfaces(type, i => i == @interface || @interface.IsGenericTypeDefinition && i.IsGenericType && i.GetGenericTypeDefinition() == @interface);
        public static IEnumerable<Type> FindInterfaces(this Type type, Predicate<Type> interfacePredicate) =>
            type
            .FindInterfaces((i, c) => interfacePredicate(i), null);

        public static bool Inherits(this Type type, Type @class) =>
            @class.IsAssignableFrom(type)
            || @class.IsGenericTypeDefinition && type.HasBaseClass(bc => bc.IsGenericType && bc.GetGenericTypeDefinition() == @class);
        public static bool HasBaseClass(this Type type, Func<Type, bool> baseClassPredicate) =>
            type.GetBaseClassesChain().Any(baseClassPredicate);

        public static Type GetBaseClass(this Type type, Type @class)
        {
            var baseClass =
                @class.IsGenericTypeDefinition
                ? type.GetBaseClass(bc => bc.IsGenericType ? bc.GetGenericTypeDefinition() == @class : bc == @class)
                : type.GetBaseClass(bc => bc == @class);
            if (baseClass != null && type.IsGenericTypeDefinition)
                baseClass = baseClass.GetGenericTypeDefinition();
            return baseClass;
        }
        public static Type GetBaseClass(this Type type, Func<Type, bool> baseClassPredicate) =>
            type.GetBaseClassesChain().FirstOrDefault(baseClassPredicate);
        public static IEnumerable<Type> GetBaseClassesChain(this Type type) =>
            type.IsClass
            ? new BaseTypesChainEnumerable(type)
            : Enumerable.Empty<Type>();

        public static bool IsNullable(this Type type) =>
            Nullable.GetUnderlyingType(type) != null;

        public static MethodInfo GetSingleMethodByDescription(this Type type, string description) =>
            type
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Single(e => string.Equals(e.GetCustomAttribute<DescriptionAttribute>(false)?.Description, description, StringComparison.Ordinal));

        public static bool HasParameterlessConstructor(this Type type) =>
            type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[0], null) != null;
    }
}
