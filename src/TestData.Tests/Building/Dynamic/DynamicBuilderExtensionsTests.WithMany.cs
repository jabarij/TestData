using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderExtensionsTests
    {
        public class WithMany : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withValue = () => DynamicBuilderExtensions.WithMany(builder, e => e.EnumerableProperty, 2, idx => idx.ToString());

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithMany<TestClass, string>(builderMock.Object, null, 2, idx => idx.ToString());

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithMany(builderMock.Object, e => e.EnumerableFunction(), 2, idx => idx.ToString());

                // assert
                var exception = withValue.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(-657)]
            public void CountLowerThanOrEqualZero_ShouldThrow(int count)
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithMany(builderMock.Object, e => e.EnumerableFunction(), count, idx => idx.ToString());

                // assert
                var exception = withValue.Should().Throw<ArgumentOutOfRangeException>().And;
                exception.ActualValue.Should().Be(count);
            }

            [Fact]
            public void ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                var builder = DynamicBuilderExtensions.WithMany(builderMock.Object, e => e.EnumerableProperty, 2, idx => idx.ToString());

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<string>>(en => en.SequenceEqual(new[] { "0", "1" }))), Times.Once);
            }

            public class TestClass
            {
                public IEnumerable<string> EnumerableProperty { get; set; }
                public IEnumerable<string> EnumerableFunction() => null;
            }
        }
    }
}
