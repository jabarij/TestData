using TestData.Building.Dynamic;
using TestData.Building.Standard;

namespace TestData.Building
{
    public static class Build
    {
        public static DynamicBuilder<T> Dynamically<T>(T template, bool fixedInstance = false) where T : class
        {
            var builder =
                fixedInstance
                ? new DynamicBuilder<T>(new FixedInstanceFactory<T>(template))
                : new DynamicBuilder<T>();
            if (!fixedInstance && template != null)
                builder.OverwriteWithTemplate(template);
            return builder;
        }
    }
}
