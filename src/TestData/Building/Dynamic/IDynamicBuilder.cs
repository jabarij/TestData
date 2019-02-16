namespace TestData.Building.Dynamic
{
    public interface IDynamicBuilder<T> : IBuilder<T>
    {
        bool IsOverwritten(string name);
        void Overwrite<TProperty>(string name, TProperty value);
        TProperty GetOverwrittenValue<TProperty>(string name);
        void OverwriteAll(T template);
    }
}