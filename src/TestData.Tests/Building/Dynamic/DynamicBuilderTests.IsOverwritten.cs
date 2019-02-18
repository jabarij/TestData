using FluentAssertions;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderTests
    {
        public class IsOverwritten : DynamicBuilderTests
        {
            [Fact]
            public void NotOverwritten_ShouldReturnFalse()
            {
                // arrange
                var sut = new DynamicBuilder<SomeClass>();

                // act
                bool result = sut.IsOverwritten(nameof(SomeClass.Int32Property));

                // assert
                result.Should().BeFalse();
            }

            [Fact]
            public void Overwritten_ShouldReturnTrue()
            {
                // arrange
                var sut = new DynamicBuilder<SomeClass>();
                sut.Overwrite(nameof(SomeClass.Int32Property), 1);

                // act
                bool result = sut.IsOverwritten(nameof(SomeClass.Int32Property));

                // assert
                result.Should().BeTrue();
            }

            class SomeClass
            {
                public int Int32Property { get; set; }
                public string StringProperty { get; set; }
            }
        }
    }
}
