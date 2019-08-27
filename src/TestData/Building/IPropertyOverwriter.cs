namespace TestData.Building
{
    public interface IPropertyOverwriter : IValueSetter, IValueGetter
    {
        bool IsOverwritten { get; }
    }
}