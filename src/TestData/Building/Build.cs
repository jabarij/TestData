using TestData.Building.Dynamic;
using TestData.Building.Standard;

namespace TestData.Building
{
    public static class Build
    {
        public static DynamicBuilder<T> Dynamically<T>() => new DynamicBuilder<T>();
        public static DynamicBuilder<T> Dynamically<T>(T template)
        {
            var builder = Dynamically<T>();
            if (template != null)
                builder.OverwriteAll(template);
            return builder;
        }
        public static DynamicBuilder<T> DynamicallyForFixedInstance<T>(T instance) where T : class => new DynamicBuilder<T>(new FixedInstanceFactory<T>(instance));
    }
}
