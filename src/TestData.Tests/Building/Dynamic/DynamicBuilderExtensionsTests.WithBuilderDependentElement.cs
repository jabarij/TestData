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
        public class WithBuilderDependentElement : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withBuilderDependentElement = () => DynamicBuilderExtensions.WithBuilderDependentElement(builder, e => e.EnumerableProperty, p => 1);

                // assert
                withBuilderDependentElement.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullPropertyExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentElement = () => DynamicBuilderExtensions.WithBuilderDependentElement(builderMock.Object, null, p => 1);

                // assert
                withBuilderDependentElement.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullGetElement_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentElement = () => DynamicBuilderExtensions.WithBuilderDependentElement(builderMock.Object, e => e.EnumerableProperty, null);

                // assert
                withBuilderDependentElement.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentElement = () => DynamicBuilderExtensions.WithBuilderDependentElement(builderMock.Object, e => e.EnumerableFunction(), p => 1);

                // assert
                var exception = withBuilderDependentElement.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void NullEnumerable_ShouldOverwriteWithNewEnumerableWithOneElement()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.EnumerableProperty))).Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue<IEnumerable<int>>(nameof(TestClass.EnumerableProperty)))
                    .Returns((IEnumerable<int>)null);
                int minValue = 1;
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.MinValueProperty))).Returns(true);
                builderMock.Setup(e => e.GetOverwrittenValue<int>(nameof(TestClass.MinValueProperty))).Returns(minValue);
                int expectedValue = minValue + 1;

                // act
                var builder = DynamicBuilderExtensions.WithBuilderDependentElement(builderMock.Object, e => e.EnumerableProperty, obj => obj.GetOverwrittenValue(e => e.MinValueProperty) + 1);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(new[] { expectedValue }))), Times.Once);
            }

            [Fact]
            public void NonCollectionEnumerableWithBuilderDependentElements_ShouldOverwriteWithEnumerableCombinedWithNewElement()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.EnumerableProperty))).Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue<IEnumerable<int>>(nameof(TestClass.EnumerableProperty)))
                    .Returns(new[] { 1, 2 });
                int minValue = 1;
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.MinValueProperty))).Returns(true);
                builderMock.Setup(e => e.GetOverwrittenValue<int>(nameof(TestClass.MinValueProperty))).Returns(minValue);
                int expectedValue = minValue + 1;

                // act
                var builder = DynamicBuilderExtensions.WithBuilderDependentElement(builderMock.Object, e => e.EnumerableProperty, obj => obj.GetOverwrittenValue(e => e.MinValueProperty) + 1);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(new[] { 1, 2, expectedValue }))), Times.Once);
            }

            [Fact]
            public void CollectionEnumerableWithBuilderDependentElements_ShouldSetSameEnumerableWithNewElementAdded()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                var collectionMock = new Mock<ICollection<int>>();
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.EnumerableProperty))).Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue<IEnumerable<int>>(nameof(TestClass.EnumerableProperty)))
                    .Returns(collectionMock.Object);
                int minValue = 2;
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.MinValueProperty))).Returns(true);
                builderMock.Setup(e => e.GetOverwrittenValue<int>(nameof(TestClass.MinValueProperty))).Returns(minValue);
                int expectedValue = minValue + 1;

                // act
                var builder = DynamicBuilderExtensions.WithBuilderDependentElement(builderMock.Object, e => e.EnumerableProperty, obj => obj.GetOverwrittenValue(e => e.MinValueProperty) + 1);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => ReferenceEquals(en, collectionMock.Object))), Times.Once);
                collectionMock.Verify(e => e.Add(expectedValue), Times.Once);
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
