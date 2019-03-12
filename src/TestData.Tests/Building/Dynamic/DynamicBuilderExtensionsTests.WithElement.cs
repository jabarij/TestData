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
        public class WithElement : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withValue = () => DynamicBuilderExtensions.WithElement(builder, e => e.EnumerableProperty, 1);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithElement(builderMock.Object, null, 1);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithElement(builderMock.Object, e => e.EnumerableFunction(), 1);

                // assert
                var exception = withValue.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void NullEnumerable_ShouldOverwriteWithNewEnumerableWithOneElement()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock
                    .Setup(e => e.GetOverwrittenValue<IEnumerable<int>>(nameof(TestClass.EnumerableProperty)))
                    .Returns((IEnumerable<int>)null);
                int expectedValue = 1;

                // act
                var builder = DynamicBuilderExtensions.WithElement(builderMock.Object, e => e.EnumerableProperty, expectedValue);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(new[] { expectedValue }))), Times.Once);
            }

            [Fact]
            public void NonCollectionEnumerableWithElements_ShouldOverwriteWithEnumerableCombinedWithNewElement()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.EnumerableProperty))).Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue<IEnumerable<int>>(nameof(TestClass.EnumerableProperty)))
                    .Returns(new[] { 1, 2 });
                int expectedValue = 3;

                // act
                var builder = DynamicBuilderExtensions.WithElement(builderMock.Object, e => e.EnumerableProperty, expectedValue);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(new[] { 1, 2, expectedValue }))), Times.Once);
            }

            [Fact]
            public void CollectionEnumerableWithElements_ShouldSetSameEnumerableWithNewElementAdded()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                var collectionMock = new Mock<ICollection<int>>();
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.EnumerableProperty))).Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue<IEnumerable<int>>(nameof(TestClass.EnumerableProperty)))
                    .Returns(collectionMock.Object);
                int expectedValue = 3;

                // act
                var builder = DynamicBuilderExtensions.WithElement(builderMock.Object, e => e.EnumerableProperty, expectedValue);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => ReferenceEquals(en, collectionMock.Object))), Times.Once);
                collectionMock.Verify(e => e.Add(expectedValue), Times.Once);
            }

            public class TestClass
            {
                public IEnumerable<int> EnumerableProperty { get; set; }
                public IEnumerable<int> EnumerableFunction() => null;
            }
        }
    }
}
