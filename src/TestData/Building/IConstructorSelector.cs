using System.Reflection;

namespace TestData.Building
{
    public interface IConstructorSelector<T>
    {
        ConstructorInfo SelectConstructor();
    }
}