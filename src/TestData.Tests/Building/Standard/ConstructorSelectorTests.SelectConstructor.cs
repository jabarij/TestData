using FluentAssertions;
using Xunit;

namespace TestData.Building.Standard
{
    partial class ConstructorSelectorTests
    {
        public class SelectConstructor : ConstructorSelectorTests
        {
            [Theory]
            [InlineData(ConstructorSelection.LeastParameters, 1)]
            [InlineData(ConstructorSelection.MostParameters, 3)]
            public void LeastParameters_ShouldCreateWithDefaultConstructorSelection(ConstructorSelection constructorSelection, int expectedConstructorParamsCount)
            {
                // arrange
                var sut = new ConstructorSelector<SomeClass>(constructorSelection);

                // act
                var constructor = sut.SelectConstructor();

                // assert
                constructor.Should().NotBeNull();
                constructor.GetParameters().Should().HaveCount(expectedConstructorParamsCount);
            }

            class SomeClass
            {
                public SomeClass(int singleParam) { }
                public SomeClass(int firstParam, int secondParam) { }
                public SomeClass(int firstParam, int secondParam, int thirdParam) { }
            }
        }
    }
}
