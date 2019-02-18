using FluentAssertions;
using Xunit;

namespace TestData.Building.Standard
{
    partial class FixedInstanceFactoryTests
    {
        public class Create : FixedInstanceFactoryTests
        {
            [Fact]
            public void ShouldReturnAssignedInstance()
            {
                // arrange
                var sut = new FixedInstanceFactory<SomeClass>(new SomeClass());

                // act
                var actual = sut.Create(null);

                // assert
                actual.Should().BeSameAs(sut.Instance);
            }

            class SomeClass
            {
            }
        }
    }
}
