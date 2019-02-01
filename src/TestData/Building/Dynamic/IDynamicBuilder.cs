namespace TestData.Building.Dynamic
{
    public interface IDynamicBuilder<T> : IBuilder<T>
    {
        void Overwrite<TProperty>(string name, TProperty value);
        void OverwriteAll(T template);
        IDynamicBuilder<TChild> CreateChildBuilder<TChild>();
    }
}