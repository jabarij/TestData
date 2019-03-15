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
        public class WithBuilderDependentElements : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withBuilderDependentElements = () => DynamicBuilderExtensions.WithBuilderDependentElements(builder, e => e.EnumerableProperty, objBuilder => new List<int> { 1, 2 });

                // assert
                withBuilderDependentElements.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullPropertyExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentElements = () => DynamicBuilderExtensions.WithBuilderDependentElements(builderMock.Object, null, objBuilder => new List<int> { 1, 2 });

                // assert
                withBuilderDependentElements.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullGetElements_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentElements = () => DynamicBuilderExtensions.WithBuilderDependentElements(builderMock.Object, e => e.EnumerableProperty, null);

                // assert
                withBuilderDependentElements.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentElements = () => DynamicBuilderExtensions.WithBuilderDependentElements(builderMock.Object, e => e.EnumerableFunction(), objBuilder => new List<int> { 1, 2 });

                // assert
                var exception = withBuilderDependentElements.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void NullEnumerable_ShouldOverwriteWithNewEnumerableWithGivenElement()
            {
                // arrange
                int additionalElement = 3;
                var additionalElements = new List<int> { 4, 5 };
                var expectedElements = new List<int> { additionalElement }.Concat(additionalElements);
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.SomeValueProperty))).Returns(true);
                builderMock.Setup(e => e.GetOverwrittenValue<int>(nameof(TestClass.SomeValueProperty))).Returns(additionalElement);
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.EnumerableProperty))).Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue<IEnumerable<int>>(nameof(TestClass.EnumerableProperty)))
                    .Returns((IEnumerable<int>)null);

                // act
                var builder = DynamicBuilderExtensions.WithBuilderDependentElements(builderMock.Object, e => e.EnumerableProperty, obj => new List<int> { obj.GetOverwrittenValue(e => e.SomeValueProperty) }.Concat(additionalElements));

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(expectedElements))), Times.Once);
            }

            [Fact]
            public void NonNullEnumerable_ShouldOverwriteWithEnumerableCombinedWithNewElement()
            {
                // arrange
                var originalElements = new List<int> { 1, 2 };
                int additionalElement = 3;
                var additionalElements = new List<int> { 4, 5 };
                var expectedElements = originalElements.Concat(new List<int> { additionalElement }).Concat(additionalElements);
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.SomeValueProperty))).Returns(true);
                builderMock.Setup(e => e.GetOverwrittenValue<int>(nameof(TestClass.SomeValueProperty))).Returns(additionalElement);
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.EnumerableProperty))).Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue<IEnumerable<int>>(nameof(TestClass.EnumerableProperty)))
                    .Returns(originalElements);

                // act
                var builder = DynamicBuilderExtensions.WithBuilderDependentElements(builderMock.Object, e => e.EnumerableProperty, obj => new List<int> { obj.GetOverwrittenValue(e => e.SomeValueProperty) }.Concat(additionalElements));

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<int>>(en => en.SequenceEqual(expectedElements))), Times.Once);
            }

            public class TestClass
            {
                public int SomeValueProperty { get; set; }
                public IEnumerable<int> EnumerableProperty { get; set; }
                public IEnumerable<int> EnumerableFunction() => null;
            }
        }
    }
}
