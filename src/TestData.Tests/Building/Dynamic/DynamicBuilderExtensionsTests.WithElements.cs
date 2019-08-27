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
        public class WithElements : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withElements = () => DynamicBuilderExtensions.WithElements(builder, e => e.EnumerableProperty, new List<int> { 1, 2 });

                // assert
                withElements.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullPropertyExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withElements = () => DynamicBuilderExtensions.WithElements(builderMock.Object, null, new List<int> { 1, 2 });

                // assert
                withElements.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withElements = () => DynamicBuilderExtensions.WithElements(builderMock.Object, e => e.EnumerableFunction(), new List<int> { 1, 2 });

                // assert
                var exception = withElements.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void NullEnumerable_ShouldOverwriteWithNewEnumerableWithOneElement()
            {
                // arrange
                var additionalElements = new List<int> { 3, 4 };
                var expectedElements = additionalElements;
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.EnumerableProperty))).Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue(nameof(TestClass.EnumerableProperty)))
                    .Returns((IEnumerable<int>)null);

                // act
                var builder = DynamicBuilderExtensions.WithElements(builderMock.Object, e => e.EnumerableProperty, additionalElements);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(expectedElements))), Times.Once);
            }

            [Fact]
            public void NonNullEnumerable_ShouldOverwriteWithEnumerableCombinedWithNewElement()
            {
                // arrange
                var originalElements = new List<int> { 1, 2 };
                var additionalElements = new List<int> { 3, 4 };
                var expectedElements = originalElements.Concat(additionalElements);
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.EnumerableProperty))).Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue(nameof(TestClass.EnumerableProperty)))
                    .Returns(originalElements);

                // act
                var builder = DynamicBuilderExtensions.WithElements(builderMock.Object, e => e.EnumerableProperty, additionalElements);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(expectedElements))), Times.Once);
            }

            public class TestClass
            {
                public IEnumerable<int> EnumerableProperty { get; set; }
                public IEnumerable<int> EnumerableFunction() => null;
            }
        }
    }
}
