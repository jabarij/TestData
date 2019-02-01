namespace TestData.Building
{
    public interface IBuilder
    {
        object Build();
        bool IsOverwritten();
    }
    public interface IBuilder<out T> : IBuilder
    {
        new T Build();
    }
}