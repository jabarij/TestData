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
        public class WithBuilderDependentMany : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withBuilderDependentMany = () => DynamicBuilderExtensions.WithBuilderDependentMany(builder, e => e.EnumerableProperty, 2, (objBuilder, idx) => idx.ToString());

                // assert
                withBuilderDependentMany.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullPropertyExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentMany = () => DynamicBuilderExtensions.WithBuilderDependentMany(builderMock.Object, null, 2, (objBuilder, idx) => idx.ToString());

                // assert
                withBuilderDependentMany.Should().Throw<ArgumentNullException>();
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
                Action withBuilderDependentMany = () => DynamicBuilderExtensions.WithBuilderDependentMany(builderMock.Object, e => e.EnumerableFunction(), count, (objBuilder, idx) => idx.ToString());

                // assert
                var exception = withBuilderDependentMany.Should().Throw<ArgumentOutOfRangeException>().And;
                exception.ActualValue.Should().Be(count);
            }

            [Fact]
            public void NullElementFactory_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentMany = () => DynamicBuilderExtensions.WithBuilderDependentMany(builderMock.Object, e => e.EnumerableProperty, 2, null);

                // assert
                withBuilderDependentMany.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withBuilderDependentMany = () => DynamicBuilderExtensions.WithBuilderDependentMany(builderMock.Object, e => e.EnumerableFunction(), 2, (objBuilder, idx) => idx.ToString());

                // assert
                var exception = withBuilderDependentMany.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                string prefix = "element";
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.Prefix))).Returns(true);
                builderMock.Setup(e => e.GetOverwrittenValue(nameof(TestClass.Prefix))).Returns(prefix);

                // act
                var builder = DynamicBuilderExtensions.WithBuilderDependentMany(builderMock.Object, e => e.EnumerableProperty, 2, (objBuilder, idx) => objBuilder.GetOverwrittenValue(e => e.Prefix) + idx.ToString());

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.EnumerableProperty), It.Is<IEnumerable<string>>(en => en.SequenceEqual(new[] { "element0", "element1" }))), Times.Once);
            }

            public class TestClass
            {
                public string Prefix { get; set; }
                public IEnumerable<string> EnumerableProperty { get; set; }
                public IEnumerable<string> EnumerableFunction() => null;
            }
        }
    }
}
