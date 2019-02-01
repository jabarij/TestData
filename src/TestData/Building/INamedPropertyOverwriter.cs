namespace TestData.Building
{
    public interface INamedPropertyOverwriter : IPropertyOverwriter
    {
        string PropertyName { get; }
    }
}