namespace TestData.Building
{
    public interface IBuilder
    {
        object Build();
    }

    public interface IBuilder<out T> : IBuilder
    {
        new T Build();
    }
}