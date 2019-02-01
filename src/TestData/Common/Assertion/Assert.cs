using System;
using System.Collections.Generic;
using TestData.Common.Collections;

namespace TestData.Common.Assertion
{
    static class Assert
    {
        public static T IsGreaterThan<T>(T value, T min, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) <= 0)
                throw new ArgumentOutOfRangeException(paramName, value, $"Expected param to be greater than {min}.");
            return value;
        }
        public static T IsGreaterThanOrEqual<T>(T value, T min, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                throw new ArgumentOutOfRangeException(paramName, value, $"Expected param to be greater than or equal to {min}.");
            return value;
        }
        public static T IsLowerThan<T>(T value, T min, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) >= 0)
                throw new ArgumentOutOfRangeException(paramName, value, $"Expected param to be lower than {min}.");
            return value;
        }
        public static T IsLowerThanOrEqual<T>(T value, T min, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) > 0)
                throw new ArgumentOutOfRangeException(paramName, value, $"Expected param to be lower than or equal to {min}.");
            return value;
        }
        public static T IsInRange<T>(T value, T min, T max, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException(paramName, value, $"Expected param to be inclusively in range {min}-{max}.");
            return value;
        }
        public static T IsBetween<T>(T value, T min, T max, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) <= 0 || value.CompareTo(max) >= 0)
                throw new ArgumentOutOfRangeException(paramName, value, $"Expected param to be between {min} and {max}.");
            return value;
        }

        public static T? IsNullOrGreaterThan<T>(T? value, T min, string paramName) where T : struct, IComparable<T> =>
            value.HasValue ? IsGreaterThan(value.Value, min, paramName) : value;
        public static T? IsNullOrGreaterThanOrEqual<T>(T? value, T min, string paramName) where T : struct, IComparable<T> =>
            value.HasValue ? IsGreaterThanOrEqual(value.Value, min, paramName) : value;
        public static T? IsNullOrLowerThan<T>(T? value, T min, string paramName) where T : struct, IComparable<T> =>
            value.HasValue ? IsLowerThan(value.Value, min, paramName) : value;
        public static T? IsNullOrLowerThanOrEqual<T>(T? value, T min, string paramName) where T : struct, IComparable<T> =>
            value.HasValue ? IsLowerThanOrEqual(value.Value, min, paramName) : value;
        public static T? IsNullOrInRange<T>(T? value, T min, T max, string paramName) where T : struct, IComparable<T> =>
            value.HasValue ? IsInRange(value.Value, min, max, paramName) : value;
        public static T? IsNullOrBetween<T>(T? value, T min, T max, string paramName) where T : struct, IComparable<T> =>
            value.HasValue ? IsBetween(value.Value, min, max, paramName) : value;

        public static T? IsNotNullAndGreaterThan<T>(T? value, T min, string paramName) where T : struct, IComparable<T> =>
            IsGreaterThan(IsNotNull(value, paramName).Value, min, paramName);
        public static T? IsNotNullAndGreaterThanOrEqual<T>(T? value, T min, string paramName) where T : struct, IComparable<T> =>
            IsGreaterThanOrEqual(IsNotNull(value, paramName).Value, min, paramName);
        public static T? IsNotNullAndLowerThan<T>(T? value, T min, string paramName) where T : struct, IComparable<T> =>
            IsLowerThan(IsNotNull(value, paramName).Value, min, paramName);
        public static T? IsNotNullAndLowerThanOrEqual<T>(T? value, T min, string paramName) where T : struct, IComparable<T> =>
            IsLowerThanOrEqual(IsNotNull(value, paramName).Value, min, paramName);
        public static T? IsNotNullAndInRange<T>(T? value, T min, T max, string paramName) where T : struct, IComparable<T> =>
            IsInRange(IsNotNull(value, paramName).Value, min, max, paramName);
        public static T? IsNotNullAndBetween<T>(T? value, T min, T max, string paramName) where T : struct, IComparable<T> =>
            IsBetween(IsNotNull(value, paramName).Value, min, max, paramName);

        public static T IsNotNull<T>(T value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
            return value;
        }
        public static T IsNotNull<T>(T value, Func<Exception> exception)
        {
            if (value == null)
                throw exception();
            return value;
        }

        public static string IsNotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(paramName);
            return value;
        }
        public static string IsNotNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(paramName);
            return value;
        }

        public static IEnumerable<T> IsNotNullOrEmpty<T>(IEnumerable<T> collection, string paramName) =>
            IsNotNullOrEmpty(collection, () => new ArgumentNullException(paramName));
        public static IEnumerable<T> IsNotNullOrEmpty<T>(IEnumerable<T> collection, Func<Exception> exception = null)
        {
            if (collection.IsNullOrEmpty())
            {
                var ex = exception?.Invoke() ?? new ArgumentNullException(nameof(collection));
                throw ex;
            }

            return collection;
        }
        public static IEnumerable<T> HasAtLeast<T>(IEnumerable<T> collection, int count, Func<Exception> exception) =>
            collection.HasAtLeast(count)
            ? collection
            : throw exception();
        public static IEnumerable<T> HasAtMost<T>(IEnumerable<T> collection, int count, Func<Exception> exception) =>
            collection.HasAtMost(count)
            ? collection
            : throw exception();
    }
}
