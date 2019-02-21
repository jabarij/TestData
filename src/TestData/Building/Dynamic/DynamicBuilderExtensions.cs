using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TestData.Building.Dynamic
{
    public static class DynamicBuilderExtensions
    {
        public static IDynamicBuilder<T> WithNull<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property) where TProperty : class =>
            WithValue(builder, property, null);

        public static IDynamicBuilder<T> WithEmpty<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> property) =>
            WithValue(builder, property, Enumerable.Empty<TElement>());

        public static IDynamicBuilder<T> WithEmpty<T>(this IDynamicBuilder<T> builder, Expression<Func<T, string>> property) =>
            WithValue(builder, property, string.Empty);

        public static IDynamicBuilder<T> WithMany<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> property, int count, Func<int, TElement> elementFactory)
        {
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count), count, "Elements count must be greater than zero.");
            if (elementFactory == null) throw new ArgumentNullException(nameof(elementFactory));
            var elements = Enumerable.Range(0, count).Select(elementFactory);
            return WithValue(builder, property, elements);
        }

        public static IDynamicBuilder<T> WithDefault<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property) =>
            WithValue(builder, property, default(TProperty));

        public static IDynamicBuilder<T> WithValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, TProperty value)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (property.Body.NodeType != ExpressionType.MemberAccess)
                Error.Raise(new ArgumentException("Only member access expressions are allowed.", nameof(property)),
                    Errors.OnlyMemberAccessExpressionAreAllowed);
            string name = ((MemberExpression)property.Body).Member.Name;
            builder.Overwrite(name, value);
            return builder;
        }

        public static IDynamicBuilder<T> WithDependentValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, Func<IDynamicBuilder<T>, TProperty> getValue)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (getValue == null) throw new ArgumentNullException(nameof(getValue));
            if (property.Body.NodeType != ExpressionType.MemberAccess)
                Error.Raise(new ArgumentException("Only member access expressions are allowed.", nameof(property)),
                    Errors.OnlyMemberAccessExpressionAreAllowed);
            string name = ((MemberExpression)property.Body).Member.Name;
            builder.Overwrite(name, getValue(builder));
            return builder;
        }

        public static TProperty GetOverwrittenValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (property.Body.NodeType != ExpressionType.MemberAccess)
                Error.Raise(new ArgumentException("Only member access expressions are allowed.", nameof(property)),
                    Errors.OnlyMemberAccessExpressionAreAllowed);
            string name = ((MemberExpression)property.Body).Member.Name;
            return
                builder.IsOverwritten(name)
                ? builder.GetOverwrittenValue<TProperty>(name)
                : default(TProperty);
        }

    }
}
