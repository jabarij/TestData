using TestData.Building.Dynamic;

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
    }
}
