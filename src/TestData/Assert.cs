using System;

namespace TestData
{
    internal static class Assert
    {
        public static T IsNotNull<T>(T value, string paramName)
        {
            if (ReferenceEquals(value, null))
                Error.Raise(new ArgumentNullException(paramName), Errors.ArgumentIsNull);
            return value;
        }

        public static string IsNotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                Error.Raise(new ArgumentNullException(paramName), Errors.ArgumentIsNullOrEmpty);
            return value;
        }

        public static string IsNotNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                Error.Raise(new ArgumentNullException(paramName), Errors.ArgumentIsNullOrWhiteSpace);
            return value;
        }

        public static T IsOfType<T>(T value, Type type, string paramName, bool acceptNull = true)
        {
            var valueType = value?.GetType();
            if (valueType != null)
            {
                if (!type.IsAssignableFrom(valueType))
                    Error.Raise(new ArgumentException("", paramName), Errors.ArgumentIsOfUnexpectedType);
                return value;
            }

            if (!acceptNull)
                IsNotNull(value, paramName);

            return value;
        }
    }
}