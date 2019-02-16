namespace TestData.Building
{
    public interface IOverwritableBuilder : IBuilder
    {
        bool IsOverwritten();
    }
    public interface IOverwritableBuilder<out T> : IBuilder<T>, IOverwritableBuilder
    {
    }
}