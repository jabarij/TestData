using System;
using System.Linq.Expressions;

namespace TestData.Building.Dynamic
{
    public static class DynamicBuilderExtensions
    {
        public static IDynamicBuilder<T> WithValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, TProperty value)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (property.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("Only member access expressions are allowed.", nameof(property));
            string name = ((MemberExpression)property.Body).Member.Name;
            builder.Overwrite(name, value);
            return builder;
        }

        public static IDynamicBuilder<T> WithDependentValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property, Func<IDynamicBuilder<T>, TProperty> getValue)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (property.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("Only member access expressions are allowed.", nameof(property));
            string name = ((MemberExpression)property.Body).Member.Name;
            builder.Overwrite(name, getValue(builder));
            return builder;
        }

        public static TProperty GetOverwrittenValue<T, TProperty>(this IDynamicBuilder<T> builder, Expression<Func<T, TProperty>> property)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (property.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("Only member access expressions are allowed.", nameof(property));
            string name = ((MemberExpression)property.Body).Member.Name;
            return
                builder.IsOverwritten(name)
                ? builder.GetOverwrittenValue<TProperty>(name)
                : default(TProperty);
        }

    }
}
