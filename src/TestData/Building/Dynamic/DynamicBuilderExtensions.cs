using System;
using System.Linq.Expressions;

namespace TestData.Building.Dynamic
{
    public static class DynamicBuilderExtensions
    {
        public static IDynamicBuilder<T> With<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, TProperty value)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (property.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("Only member access expressions are allowed.", nameof(property));
            string name = ((MemberExpression)property.Body).Member.Name;
            builder.Overwrite(name, value);
            return builder;
        }

        public static IDynamicBuilder<T> With<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, Action<IDynamicBuilder<TProperty>> build)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (property.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("Only member access expressions are allowed.", nameof(property));
            string name = ((MemberExpression)property.Body).Member.Name;
            var propertyBuilder = builder.CreateChildBuilder<TProperty>();
            build(propertyBuilder);
            builder.Overwrite(name, propertyBuilder.Build());
            return builder;
        }
    }
}
