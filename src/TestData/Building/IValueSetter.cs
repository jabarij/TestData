namespace TestData.Building
{
    public interface IValueSetter
    {
        void SetValue(object value);
    }

    public interface IValueSetter<in T> : IValueSetter
    {
        T Value { set; }
    }
}