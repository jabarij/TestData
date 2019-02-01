using FluentAssertions;
using Xunit;

namespace TestData.Building.Standard
{
    partial class ConstructorSelectorTests
    {
        public class Constructor : ConstructorSelectorTests
        {
            [Fact]
            public void NoParams_ShouldCreateWithDefaultConstructorSelection()
            {
                // arrange
                ConstructorSelector<SomeClass> sut;

                // act
                sut = new ConstructorSelector<SomeClass>();

                // assert
                sut.ConstructorSelection.Should().Be(ConstructorSelection.Default);
            }

            [Theory]
            [InlineData(ConstructorSelection.LeastParameters)]
            [InlineData(ConstructorSelection.MostParameters)]
            public void ConstructorSelection_ShouldCreateWithGivenConstructorSelection(ConstructorSelection constructorSelection)
            {
                // arrange
                ConstructorSelector<SomeClass> sut;

                // act
                sut = new ConstructorSelector<SomeClass>(constructorSelection);

                // assert
                sut.ConstructorSelection.Should().Be(constructorSelection);

            }

            class SomeClass
            {

            }
        }
    }
}
