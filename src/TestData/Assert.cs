using System;
using TestData.Common;

namespace TestData
{
    internal static class Assert
    {
        public static T IsNotNull<T>(T value, string paramName)
        {
            if (ReferenceEquals(value, null))
                Error.Raise(new ArgumentNullException(paramName), Errors.ObjectIsNull);
            return value;
        }

        public static string IsNotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                Error.Raise(new ArgumentNullException(paramName), Errors.ObjectIsNullOrEmptyString);
            return value;
        }

        public static string IsNotNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                Error.Raise(new ArgumentNullException(paramName), Errors.ObjectIsNullOrWhiteSpace);
            return value;
        }

        public static T IsOfType<T>(T value, Type type, string paramName, bool acceptNull = true)
        {
            var valueType = value?.GetType();
            if (valueType != null)
            {
                if (!type.IsAssignableFrom(valueType))
                    Error.Raise(new ArgumentException("", paramName), Errors.ObjectIsOfUnexpectedType);
                return value;
            }

            if (!acceptNull)
                IsNotNull(value, paramName);

            return value;
        }

        public static T IsGreaterThan<T>(T value, T min, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) <= 0)
                Error.Raise(new ArgumentOutOfRangeException(paramName, value, $"Expected param to be greater than {min}."), Errors.ObjectIsOutOfRange);
            return value;
        }
        public static T IsGreaterThanOrEqual<T>(T value, T min, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                Error.Raise(new ArgumentOutOfRangeException(paramName, value, $"Expected param to be greater than or equal to {min}."), Errors.ObjectIsOutOfRange);
            return value;
        }
        public static T IsLowerThan<T>(T value, T min, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) >= 0)
                Error.Raise(new ArgumentOutOfRangeException(paramName, value, $"Expected param to be lower than {min}."), Errors.ObjectIsOutOfRange);
            return value;
        }
        public static T IsLowerThanOrEqual<T>(T value, T min, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) > 0)
                Error.Raise(new ArgumentOutOfRangeException(paramName, value, $"Expected param to be lower than or equal to {min}."), Errors.ObjectIsOutOfRange);
            return value;
        }
        public static T IsInRange<T>(T value, T min, T max, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
                Error.Raise(new ArgumentOutOfRangeException(paramName, value, $"Expected param to be inclusively in range {min}-{max}."), Errors.ObjectIsOutOfRange);
            return value;
        }
        public static T IsBetween<T>(T value, T min, T max, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) <= 0 || value.CompareTo(max) >= 0)
                Error.Raise(new ArgumentOutOfRangeException(paramName, value, $"Expected param to be between {min} and {max}."), Errors.ObjectIsOutOfRange);
            return value;
        }

        public static T IsNotDisposed<T>(T obj) where T : IExtendedDisposable =>
            IsNotDisposed(obj, e => e.IsDisposed);
        public static T IsNotDisposed<T>(T obj, Predicate<T> isDisposedPredicate) where T : IDisposable
        {
            IsNotNull(obj, nameof(obj));
            if (isDisposedPredicate(obj))
                Error.Raise(new ObjectDisposedException(obj.GetType().FullName), Errors.ObjectIsDisposed);
            return obj;
        }
    }
}