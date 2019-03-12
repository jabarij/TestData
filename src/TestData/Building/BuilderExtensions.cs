using System;

namespace TestData.Building
{
    public static class BuilderExtensions
    {
        public static TBuilder WithValue<TBuilder, TProperty>(
            this TBuilder builder,
            Func<TBuilder, IValueSetter<TProperty>> property,
            TProperty value)
           where TBuilder : IBuilder
        {
            var setter = property(builder);
            setter.Value = value;
            return builder;
        }

        public static TBuilder WithDependentValue<TBuilder, TProperty>(
            this TBuilder builder,
            Func<TBuilder, IValueSetter<TProperty>> property,
            Func<TBuilder, TProperty> getValue)
           where TBuilder : IBuilder =>
            WithValue(builder, property, getValue(builder));

        public static TParentBuilder WithValue<TParentBuilder, TChildBuilder, TProperty>(
            this TParentBuilder builder,
            Func<TParentBuilder, TChildBuilder> childBuilder,
            Func<TChildBuilder, IValueSetter<TProperty>> property,
            TProperty value)
            where TParentBuilder : IBuilder
            where TChildBuilder : IBuilder
        {
            childBuilder(builder).WithValue(property, value);
            return builder;
        }

        public static TParentBuilder WithDependentValue<TParentBuilder, TChildBuilder, TProperty>(
            this TParentBuilder builder,
            Func<TParentBuilder, TChildBuilder> childBuilder,
            Func<TChildBuilder, IValueSetter<TProperty>> property,
            Func<TParentBuilder, TChildBuilder, TProperty> getValue)
            where TParentBuilder : IBuilder
            where TChildBuilder : IBuilder =>
            WithValue(builder, childBuilder, property, getValue(builder, childBuilder(builder)));

        public static TBuilder WithAction<TBuilder>(
            this TBuilder builder,
            Action<TBuilder> action)
           where TBuilder : IBuilder
        {
            action(builder);
            return builder;
        }
    }
}
