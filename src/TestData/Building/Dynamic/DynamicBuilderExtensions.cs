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

        public static IDynamicBuilder<T> WithNull<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty?>> property) where TProperty : struct =>
            WithValue(builder, property, null);

        public static IDynamicBuilder<T> WithEmpty<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> property) =>
            WithValue(builder, property, Enumerable.Empty<TElement>());

        public static IDynamicBuilder<T> WithEmpty<T>(this IDynamicBuilder<T> builder, Expression<Func<T, string>> property) =>
            WithValue(builder, property, string.Empty);

        public static IDynamicBuilder<T> WithSingle<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, TElement element) =>
            WithValue(builder, enumerableProperty, new[] { element }.AsEnumerable());

        public static IDynamicBuilder<T> WithBuilderDependentSingle<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, Func<IDynamicBuilder<T>, TElement> getElement)
        {
            if (getElement == null) throw new ArgumentNullException(nameof(getElement));
            return WithBuilderDependentValue(builder, enumerableProperty, b => new[] { getElement(b) });
        }

        public static IDynamicBuilder<T> WithDependentSingle<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, Func<T, TElement> getElement)
        {
            if (getElement == null) throw new ArgumentNullException(nameof(getElement));
            return WithBuilderDependentSingle(builder, enumerableProperty, b => getElement(b.Build()));
        }

        public static IDynamicBuilder<T> WithElement<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, TElement element)
        {
            var value = builder.GetOverwrittenValue(enumerableProperty);
            if (value == null)
                value = new[] { element };
            else if (value is ICollection<TElement> collection && !collection.IsReadOnly)
                collection.Add(element);
            else
                value = value.Concat(new[] { element });
            return WithValue(builder, enumerableProperty, value);
        }

        public static IDynamicBuilder<T> WithBuilderDependentElement<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, Func<IDynamicBuilder<T>, TElement> getElement)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (getElement == null) throw new ArgumentNullException(nameof(getElement));
            var element = getElement(builder);
            var value = builder.GetOverwrittenValue(enumerableProperty);
            if (value == null)
                value = new[] { element };
            else if (value is ICollection<TElement> collection && !collection.IsReadOnly)
                collection.Add(element);
            else
                value = value.Concat(new[] { element });
            return WithBuilderDependentValue(builder, enumerableProperty, b => value);
        }

        public static IDynamicBuilder<T> WithDependentElement<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, Func<T, TElement> getElement)
        {
            if (getElement == null) throw new ArgumentNullException(nameof(getElement));
            return WithBuilderDependentElement(builder, enumerableProperty, b => getElement(b.Build()));
        }

        public static IDynamicBuilder<T> WithMany<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, int count, Func<int, TElement> elementFactory)
        {
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count), count, "Elements count must be greater than zero.");
            if (elementFactory == null) throw new ArgumentNullException(nameof(elementFactory));
            var elements = Enumerable.Range(0, count).Select(elementFactory);
            return WithValue(builder, enumerableProperty, elements);
        }

        public static IDynamicBuilder<T> WithBuilderDependentMany<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, int count, Func<IDynamicBuilder<T>, int, TElement> elementFactory)
        {
            if (elementFactory == null) throw new ArgumentNullException(nameof(elementFactory));
            return WithMany(builder, enumerableProperty, count, idx => elementFactory(builder, idx));
        }

        public static IDynamicBuilder<T> WithDependentMany<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, int count, Func<T, int, TElement> elementFactory)
        {
            if (elementFactory == null) throw new ArgumentNullException(nameof(elementFactory));
            return WithMany(builder, enumerableProperty, count, idx => elementFactory(builder.Build(), idx));
        }

        public static IDynamicBuilder<TParent> WithChild<TParent, TChild>(
            this IDynamicBuilder<TParent> builder,
            Expression<Func<TParent, TChild>> childProperty,
            Func<IDynamicBuilder<TChild>, IDynamicBuilder<TChild>> buildChild)
            where TChild : class =>
            WithValue(builder, childProperty,
                (buildChild ?? throw new ArgumentNullException(nameof(buildChild))).Invoke(
                    Build.Dynamically(
                        GetOverwrittenValue(builder, childProperty))).Build());

        public static IDynamicBuilder<TParent> WithBuilderDependentChild<TParent, TChild>(
            this IDynamicBuilder<TParent> builder,
            Expression<Func<TParent, TChild>> childProperty,
            Func<IDynamicBuilder<TParent>, IDynamicBuilder<TChild>, IDynamicBuilder<TChild>> buildChild)
            where TChild : class
        {
            if (buildChild == null) throw new ArgumentNullException(nameof(buildChild));
            var childBuilder = Build.Dynamically(GetOverwrittenValue(builder, childProperty));
            var child = buildChild(builder, childBuilder).Build();
            return WithValue(builder, childProperty, child);
        }

        public static IDynamicBuilder<TParent> WithDependentChild<TParent, TChild>(
            this IDynamicBuilder<TParent> builder,
            Expression<Func<TParent, TChild>> childProperty,
            Func<TParent, IDynamicBuilder<TChild>, IDynamicBuilder<TChild>> buildChild)
            where TChild : class
        {
            if (buildChild == null) throw new ArgumentNullException(nameof(buildChild));
            return WithBuilderDependentChild(builder, childProperty, (parentBuilder, childBuilder) => buildChild(parentBuilder.Build(), childBuilder));
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

        public static IDynamicBuilder<T> WithBuilderDependentValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, Func<IDynamicBuilder<T>, TProperty> getValue)
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

        public static IDynamicBuilder<T> WithDependentValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, Func<T, TProperty> getValue)
        {
            if (getValue == null) throw new ArgumentNullException(nameof(getValue));
            return WithBuilderDependentValue(builder, property, b => getValue(b.Build()));
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
