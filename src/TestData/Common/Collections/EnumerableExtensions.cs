using System;
using System.Collections.Generic;
using System.Linq;

namespace TestData.Common.Collections
{
    static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) =>
            source == null || !source.Any();

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source) =>
            source ?? Enumerable.Empty<T>();

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source) where T : class =>
            source
            .EmptyIfNull()
            .Where(e => e != null);

        public static bool HasOne<T>(this IEnumerable<T> source)
        {
            source = source.EmptyIfNull();
            return
                source.Any()
                && !source.Skip(1).Any();
        }

        public static bool HasMany<T>(this IEnumerable<T> source) =>
            source
            .EmptyIfNull()
            .Skip(1)
            .Any();

        public static bool HasAtLeast<T>(this IEnumerable<T> source, int count) =>
            source
            .EmptyIfNull()
            .Skip(count - 1)
            .Any();

        public static bool HasAtMost<T>(this IEnumerable<T> source, int count) =>
            !source
            .EmptyIfNull()
            .Skip(count)
            .Any();

        public static IEnumerable<T> WhereIsDuplicated<T>(this IEnumerable<T> source) =>
            source
            .EmptyIfNull()
            .GroupBy(e => e)
            .Where(e => e.HasMany())
            .Select(e => e.Key);

        public static IEnumerable<TKey> WhereIsDuplicatedBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector) where T : class =>
            source
            .EmptyIfNull()
            .GroupBy(keySelector)
            .Where(e => e.HasMany())
            .Select(e => e.Key);

        public static IEnumerable<string> WhereNotNullOrEmpty(this IEnumerable<string> source) =>
            source
            .EmptyIfNull()
            .Where(e => !string.IsNullOrEmpty(e));

        public static IEnumerable<string> WhereNotNullOrWhiteSpace(this IEnumerable<string> source) =>
            source
            .EmptyIfNull()
            .Where(e => !string.IsNullOrWhiteSpace(e));

        public static SortedDictionary<TKey, TSource> ToSortedDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
            new SortedDictionary<TKey, TSource>(source.ToDictionary(keySelector));

        public static SortedDictionary<TKey, TSource> ToSortedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
            new SortedDictionary<TKey, TSource>(source.ToDictionary(keySelector), comparer);

        public static SortedDictionary<TKey, TElement> ToSortedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
            new SortedDictionary<TKey, TElement>(source.ToDictionary(keySelector, elementSelector));

        public static SortedDictionary<TKey, TElement> ToSortedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IComparer<TKey> comparer) =>
            new SortedDictionary<TKey, TElement>(source.ToDictionary(keySelector, elementSelector), comparer);
    }
}
