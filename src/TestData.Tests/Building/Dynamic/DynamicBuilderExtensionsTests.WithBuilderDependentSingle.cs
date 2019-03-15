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
        public class WithBuilderDependentSingle : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withBuilderDependentSingle = () => DynamicBuilderExtensions.WithBuilderDependentSingle(builder, e => e.EnumerableProperty, p => 1);

                // assert
                withBuilderDependentSingle.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullPropertyExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentSingle = () => DynamicBuilderExtensions.WithBuilderDependentSingle(builderMock.Object, null, p => 1);

                // assert
                withBuilderDependentSingle.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullGetElement_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentSingle = () => DynamicBuilderExtensions.WithBuilderDependentSingle(builderMock.Object, e => e.EnumerableProperty, null);

                // assert
                withBuilderDependentSingle.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentSingle = () => DynamicBuilderExtensions.WithBuilderDependentSingle(builderMock.Object, e => e.EnumerableFunction(), p => 1);

                // assert
                var exception = withBuilderDependentSingle.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                int minValue = 1;
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.MinValueProperty))).Returns(true);
                builderMock.Setup(e => e.GetOverwrittenValue<int>(nameof(TestClass.MinValueProperty))).Returns(minValue);
                int expectedValue = minValue + 1;

                // act
                var builder = DynamicBuilderExtensions.WithBuilderDependentSingle(builderMock.Object, e => e.EnumerableProperty, objBuilder => objBuilder.GetOverwrittenValue<int>(nameof(TestClass.MinValueProperty)) + 1);

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
