using System;

namespace TestData.Building
{
    public static class BuilderExtensions
    {
        public static TBuilder With<TBuilder, TProperty>(this TBuilder builder, Func<TBuilder, IValueSetter<TProperty>> property, Func<TBuilder, TProperty> value)
           where TBuilder : IBuilder
        {
            var setter = property(builder);
            setter.Value = value(builder);
            return builder;
        }

        public static TBuilder With<TBuilder, TProperty>(this TBuilder builder, Func<TBuilder, IValueSetter<TProperty>> property, TProperty value)
           where TBuilder : IBuilder
        {
            var setter = property(builder);
            setter.Value = value;
            return builder;
        }

        public static TParentBuilder With<TParentBuilder, TChildBuilder, TProperty>(this TParentBuilder builder, Func<TParentBuilder, TChildBuilder> childBuilder, Func<TChildBuilder, IValueSetter<TProperty>> property, TProperty value)
            where TParentBuilder : IBuilder
            where TChildBuilder : IBuilder
        {
            childBuilder(builder).With(property, value);
            return builder;
        }

        public static TBuilder Do<TBuilder>(this TBuilder builder, Action<TBuilder> action)
           where TBuilder : IBuilder
        {
            action(builder);
            return builder;
        }
    }
}
