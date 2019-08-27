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
        public class WithDependentElement : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withDependentElement = () => DynamicBuilderExtensions.WithDependentElement(builder, e => e.EnumerableProperty, p => 1);

                // assert
                withDependentElement.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullPropertyExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withDependentElement = () => DynamicBuilderExtensions.WithDependentElement(builderMock.Object, null, p => 1);

                // assert
                withDependentElement.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withDependentElement = () => DynamicBuilderExtensions.WithDependentElement(builderMock.Object, e => e.EnumerableFunction(), p => 1);

                // assert
                var exception = withDependentElement.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void NullEnumerable_ShouldOverwriteWithNewEnumerableWithOneElement()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock
                    .Setup(e => e.GetOverwrittenValue(nameof(TestClass.EnumerableProperty)))
                    .Returns((IEnumerable<int>)null);
                int minValue = 1;
                builderMock.Setup(e => e.Build()).Returns(new TestClass { MinValueProperty = minValue });
                int expectedValue = minValue + 1;

                // act
                var builder = DynamicBuilderExtensions.WithDependentElement(builderMock.Object, e => e.EnumerableProperty, obj => obj.MinValueProperty + 1);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(new List<int> { expectedValue }))), Times.Once);
            }

            [Fact]
            public void NonNullEnumerable_ShouldOverwriteWithEnumerableCombinedWithNewElement()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.EnumerableProperty))).Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue(nameof(TestClass.EnumerableProperty)))
                    .Returns(new List<int> { 1, 2 });
                int minValue = 2;
                builderMock.Setup(e => e.Build()).Returns(new TestClass { MinValueProperty = minValue });
                int expectedValue = minValue + 1;

                // act
                var builder = DynamicBuilderExtensions.WithDependentElement(builderMock.Object, e => e.EnumerableProperty, obj => obj.MinValueProperty + 1);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(new List<int> { 1, 2, expectedValue }))), Times.Once);
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
