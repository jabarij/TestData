using System;

namespace TestData.Building.Dynamic
{
    public interface IDynamicBuilder : IBuilder
    {
        bool IsOverwritten(string name);
        void Overwrite(string name, object value);
        object GetOverwrittenValue(string name);
        void OverwriteWithTemplate(object template);
        void OverwriteWithDelegate(Action<OverwritingContext> overwrite);
    }

    public interface IDynamicBuilder<T> : IDynamicBuilder, IBuilder<T>
    {
    }
}