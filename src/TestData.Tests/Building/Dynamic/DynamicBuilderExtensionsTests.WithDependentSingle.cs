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
        public class WithDependentSingle : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withDependentSingle = () => DynamicBuilderExtensions.WithDependentSingle(builder, e => e.EnumerableProperty, p => 1);

                // assert
                withDependentSingle.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullPropertyExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withDependentSingle = () => DynamicBuilderExtensions.WithDependentSingle(builderMock.Object, null, p => 1);

                // assert
                withDependentSingle.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullGetElement_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withDependentSingle = () => DynamicBuilderExtensions.WithDependentSingle(builderMock.Object, e => e.EnumerableProperty, null);

                // assert
                withDependentSingle.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withDependentSingle = () => DynamicBuilderExtensions.WithDependentSingle(builderMock.Object, e => e.EnumerableFunction(), p => 1);

                // assert
                var exception = withDependentSingle.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                int minValue = 1;
                builderMock.Setup(e => e.Build()).Returns(new TestClass { MinValueProperty = minValue });
                int expectedValue = minValue + 1;

                // act
                var builder = DynamicBuilderExtensions.WithDependentSingle(builderMock.Object, e => e.EnumerableProperty, obj => obj.MinValueProperty + 1);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(new[] { expectedValue }))), Times.Once);
            }

            public class TestClass
            {
                public int MinValueProperty { get; set; }
                public IEnumerable<int> EnumerableProperty { get; set; }
                public IEnumerable<int> EnumerableFunction() => null;
            }
        }
    }
}
