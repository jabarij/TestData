namespace TestData.Building
{
    public interface IValueGetter
    {
        object GetValue();
    }

    public interface IValueGetter<T> : IValueGetter
    {
        T Value { get; }
    }
}