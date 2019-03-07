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
        public class WithOne : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withValue = () => DynamicBuilderExtensions.WithOne(builder, e => e.EnumerableProperty, 1);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithOne(builderMock.Object, null, 1);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithOne(builderMock.Object, e => e.EnumerableFunction(), 1);

                // assert
                var exception = withValue.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                int expectedValue = 1;

                // act
                var builder = DynamicBuilderExtensions.WithOne(builderMock.Object, e => e.EnumerableProperty, expectedValue);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(new[] { expectedValue }))), Times.Once);
            }

            public class TestClass
            {
                public IEnumerable<int> EnumerableProperty { get; set; }
                public IEnumerable<int> EnumerableFunction() => null;
            }
        }
    }
}
