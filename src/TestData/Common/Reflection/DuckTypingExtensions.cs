using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestData.Common.Reflection
{
    public static class DuckTypingExtensions
    {
        public static T GetFieldValue<T>(this object obj, string fieldName, bool nonPublic = false)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrWhiteSpace(fieldName)) throw new ArgumentNullException(nameof(fieldName));

            var field = GetField(
                owner: obj,
                ownerType: obj.GetType(),
                fieldType: typeof(T),
                name: fieldName,
                nonPublic: nonPublic);

            return
                field != null
                ? (T)field.GetValue(obj)
                : default(T);
        }
        private static FieldInfo GetField(object owner, Type ownerType, Type fieldType, string name, bool nonPublic)
        {
            var fieldBinding = GetMemberBinding(owner != null, nonPublic);
            var field = ownerType
                .GetBaseClassesChain()
                .SelectMany(t => t.GetFields(fieldBinding | BindingFlags.DeclaredOnly))
                .SingleOrDefault(f => string.Equals(f.Name, name, StringComparison.Ordinal) && fieldType.IsAssignableFrom(f.FieldType));
            return field;
        }

        public static T GetPropertyValue<T>(this object obj, string propertyName, bool nonPublic = false)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));

            var property = GetProperty(
                owner: obj,
                ownerType: obj.GetType(),
                propertyType: typeof(T),
                name: propertyName,
                nonPublic: nonPublic);

            return
                property != null
                ? (T)property.GetValue(obj)
                : default(T);
        }
        private static PropertyInfo GetProperty(object owner, Type ownerType, Type propertyType, string name, bool nonPublic)
        {
            var propertyBinding = GetMemberBinding(owner != null, nonPublic) | BindingFlags.GetProperty;
            var property = ownerType
                .GetProperties(propertyBinding)
                .SingleOrDefault(f => string.Equals(f.Name, name, StringComparison.Ordinal) && propertyType.IsAssignableFrom(f.PropertyType));
            return property;
        }

        public static T GetMemberValue<T>(this object obj, string memberName, bool nonPublic = false)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrWhiteSpace(memberName)) throw new ArgumentNullException(nameof(memberName));

            var property = GetProperty(
                owner: obj,
                ownerType: obj.GetType(),
                propertyType: typeof(T),
                name: memberName,
                nonPublic: nonPublic);
            if (property != null)
                return (T)property.GetValue(obj);

            var field = GetField(
                owner: obj,
                ownerType: obj.GetType(),
                fieldType: typeof(T),
                name: memberName,
                nonPublic: nonPublic);

            return
                field != null
                ? (T)field.GetValue(obj)
                : default(T);
        }

        public static T GetFunction<T>(this object obj, string name, bool nonPublic = false)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var function = GetFunction(
                owner: obj,
                ownerType: obj.GetType(),
                delegateType: typeof(T),
                name: name,
                nonPublic: nonPublic);

            return
                function != null
                ? (T)(object)function.CreateDelegate(typeof(T), obj)
                : default(T);
        }
        private static MethodInfo GetFunction(object owner, Type ownerType, Type delegateType, string name, bool nonPublic)
        {
            if (!typeof(Delegate).IsAssignableFrom(delegateType))
                throw new InvalidCastException($"Expected type {delegateType.FullName} to be a delegate.");

            var functionBinding = GetMemberBinding(owner != null, nonPublic) | BindingFlags.InvokeMethod;
            var method = ownerType
                .GetMethods(functionBinding)
                .SingleOrDefault(m => string.Equals(m.Name, name, StringComparison.Ordinal) && MatchesDelegateType(m, delegateType));
            return method;
        }
        private static bool MatchesDelegateType(MethodInfo methodInfo, Type delegateType)
        {
            var delegateMethodInfo = delegateType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
            return
                methodInfo.ReturnType.IsAssignableFrom(delegateMethodInfo.ReturnType)
                && delegateMethodInfo.GetParameters()
                    .SequenceEqual(methodInfo.GetParameters(), new ParameterTypeComparer());
        }

        class ParameterTypeComparer : IEqualityComparer<ParameterInfo>
        {
            public bool Equals(ParameterInfo x, ParameterInfo y) => x.ParameterType.IsAssignableFrom(y.ParameterType);
            public int GetHashCode(ParameterInfo obj) => obj.GetHashCode();
        }
        private static BindingFlags GetMemberBinding(bool isInstance, bool nonPublic)
        {
            var binding = BindingFlags.Public;
            binding |=
                isInstance
                ? BindingFlags.Instance
                : BindingFlags.Static;

            if (nonPublic)
                binding |= BindingFlags.NonPublic;

            return binding;
        }
    }
}
