namespace TestData.Building
{
    public interface IPropertyOverwriter: IValueSetter, IValueGetter
    {
        bool IsOverwritten { get; }
    }

    public interface IPropertyOverwriter<T> : IPropertyOverwriter, IValueSetter<T>, IValueGetter<T>
    {
    }
}