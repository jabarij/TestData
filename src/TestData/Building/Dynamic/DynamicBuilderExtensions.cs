using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TestData.Building.Dynamic
{
    public static class DynamicBuilderExtensions
    {
        public static IDynamicBuilder<T> Overwrite<T, TProperty>(this IDynamicBuilder<T> builder, string name, TProperty value)
        {
            builder.Overwrite(name, value);
            return builder;
        }

        public static IDynamicBuilder Overwrite<TProperty>(this IDynamicBuilder builder, string name, TProperty value)
        {
            builder.Overwrite(name, value);
            return builder;
        }

        public static TProperty GetOverwrittenValue<TProperty>(this IDynamicBuilder builder, string name) =>
            (TProperty)builder.GetOverwrittenValue(name);

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
            Assert.IsNotNull(getElement, nameof(getElement));
            return WithBuilderDependentValue(builder, enumerableProperty, b => new[] { getElement(b) });
        }

        public static IDynamicBuilder<T> WithDependentSingle<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, Func<T, TElement> getElement)
        {
            Assert.IsNotNull(getElement, nameof(getElement));
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
            Assert.IsNotNull(builder, nameof(builder));
            Assert.IsNotNull(getElement, nameof(getElement));
            var element = getElement(builder);
            var value = builder.GetOverwrittenValue(enumerableProperty);
            if (value == null)
                value = new[] { element };
            else
                value = value.Concat(new[] { element });
            return WithBuilderDependentValue(builder, enumerableProperty, b => value);
        }

        public static IDynamicBuilder<T> WithDependentElement<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, Func<T, TElement> getElement)
        {
            Assert.IsNotNull(getElement, nameof(getElement));
            return WithBuilderDependentElement(builder, enumerableProperty, b => getElement(b.Build()));
        }

        public static IDynamicBuilder<T> WithElements<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, IEnumerable<TElement> elements)
        {
            var value = builder.GetOverwrittenValue(enumerableProperty);
            if (value == null)
                value = elements;
            else
                value = value.Concat(elements);
            return WithValue(builder, enumerableProperty, value);
        }

        public static IDynamicBuilder<T> WithBuilderDependentElements<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, Func<IDynamicBuilder<T>, IEnumerable<TElement>> getElements)
        {
            Assert.IsNotNull(getElements, nameof(getElements));
            return WithElements(builder, enumerableProperty, getElements(builder));
        }

        public static IDynamicBuilder<T> WithDependentElements<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, Func<T, IEnumerable<TElement>> getElements)
        {
            Assert.IsNotNull(builder, nameof(builder));
            Assert.IsNotNull(getElements, nameof(getElements));
            return WithElements(builder, enumerableProperty, getElements(builder.Build()));
        }

        public static IDynamicBuilder<T> WithMany<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, int count, Func<int, TElement> elementFactory)
        {
            Assert.IsGreaterThan(count, 0, nameof(count));
            Assert.IsNotNull(elementFactory, nameof(elementFactory));
            var elements = Enumerable.Range(0, count).Select(elementFactory);
            return WithValue(builder, enumerableProperty, elements);
        }

        public static IDynamicBuilder<T> WithBuilderDependentMany<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, int count, Func<IDynamicBuilder<T>, int, TElement> elementFactory)
        {
            Assert.IsNotNull(elementFactory, nameof(elementFactory));
            return WithMany(builder, enumerableProperty, count, idx => elementFactory(builder, idx));
        }

        public static IDynamicBuilder<T> WithDependentMany<T, TElement>(this IDynamicBuilder<T> builder, Expression<Func<T, IEnumerable<TElement>>> enumerableProperty, int count, Func<T, int, TElement> elementFactory)
        {
            Assert.IsNotNull(elementFactory, nameof(elementFactory));
            return WithMany(builder, enumerableProperty, count, idx => elementFactory(builder.Build(), idx));
        }

        public static IDynamicBuilder<TParent> WithChild<TParent, TChild>(
            this IDynamicBuilder<TParent> builder,
            Expression<Func<TParent, TChild>> childProperty,
            Func<IDynamicBuilder<TChild>, IDynamicBuilder<TChild>> buildChild)
            where TChild : class =>
            WithValue(builder, childProperty,
                Assert.IsNotNull(buildChild, nameof(buildChild)).Invoke(
                    Build.Dynamically(
                        GetOverwrittenValue(builder, childProperty))).Build());

        public static IDynamicBuilder<TParent> WithBuilderDependentChild<TParent, TChild>(
            this IDynamicBuilder<TParent> builder,
            Expression<Func<TParent, TChild>> childProperty,
            Func<IDynamicBuilder<TParent>, IDynamicBuilder<TChild>, IDynamicBuilder<TChild>> buildChild)
            where TChild : class
        {
            Assert.IsNotNull(buildChild, nameof(buildChild));
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
            Assert.IsNotNull(buildChild, nameof(buildChild));
            return WithBuilderDependentChild(builder, childProperty, (parentBuilder, childBuilder) => buildChild(parentBuilder.Build(), childBuilder));
        }

        public static IDynamicBuilder<T> WithDefault<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property) =>
            WithValue(builder, property, default(TProperty));

        public static IDynamicBuilder<T> WithValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, TProperty value)
        {
            Assert.IsNotNull(builder, nameof(builder));
            Assert.IsNotNull(property, nameof(property));

            string name = ExtractMemberAccessor(property).Member.Name;
            builder.Overwrite(name, value);
            return builder;
        }

        public static IDynamicBuilder<T> WithBuilderDependentValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, Func<IDynamicBuilder<T>, TProperty> getValue)
        {
            Assert.IsNotNull(builder, nameof(builder));
            Assert.IsNotNull(property, nameof(property));
            Assert.IsNotNull(getValue, nameof(getValue));

            string name = ExtractMemberAccessor(property).Member.Name;
            builder.Overwrite(name, getValue(builder));
            return builder;
        }

        public static IDynamicBuilder<T> WithDependentValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, Func<T, TProperty> getValue)
        {
            Assert.IsNotNull(getValue, nameof(getValue));
            return WithBuilderDependentValue(builder, property, b => getValue(b.Build()));
        }

        public static IDynamicBuilder<T> WithTransformedValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, Func<TProperty, TProperty> transformation)
        {
            Assert.IsNotNull(transformation, nameof(transformation));
            var value = builder.GetOverwrittenValue(property);
            var transformedValue = transformation(value);
            return WithValue(builder, property, transformedValue);
        }

        public static TProperty GetOverwrittenValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property)
        {
            Assert.IsNotNull(builder, nameof(builder));
            Assert.IsNotNull(property, nameof(property));

            string name = ExtractMemberAccessor(property).Member.Name;
            return
                builder.IsOverwritten(name)
                ? builder.GetOverwrittenValue<TProperty>(name)
                : default(TProperty);
        }

        private static MemberExpression ExtractMemberAccessor<T, TProperty>(Expression<Func<T, TProperty>> property)
        {
            var body = property.Body;
            while (body.NodeType == ExpressionType.Convert)
                body = ((UnaryExpression)body).Operand;
            if (body.NodeType != ExpressionType.MemberAccess)
                Error.Raise(new ArgumentException($"Expected expression to be a member access, but got expression of node type {body.NodeType}.", nameof(property)),
                    Errors.OnlyMemberAccessExpressionAreAllowed);
            return (MemberExpression)body;
        }
    }
}
