using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderExtensionsTests
    {
        public class WithBuilderDependentValue : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withValue = () => DynamicBuilderExtensions.WithBuilderDependentValue(builder, e => e.MinValueProperty, db => db.GetOverwrittenValue(e => e.MinValueProperty) + 1);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithBuilderDependentValue(builderMock.Object, null, db => db.GetOverwrittenValue(e => e.MinValueProperty) + 1);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullGetBuilder_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithBuilderDependentValue(builderMock.Object, e => e.MinValueProperty, null);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithBuilderDependentValue(builderMock.Object, e => e.Int32Function(), db => db.GetOverwrittenValue(e => e.MinValueProperty) + 1);

                // assert
                var exception = withValue.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                int minValue = 1;
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClass.MinValueProperty))).Returns(true);
                builderMock.Setup(e => e.GetOverwrittenValue(nameof(TestClass.MinValueProperty))).Returns(minValue);
                int expectedMaxValue = minValue + 1;

                // act
                var builder = DynamicBuilderExtensions.WithBuilderDependentValue(builderMock.Object, e => e.MaxValueProperty, lenderBuilder => lenderBuilder.GetOverwrittenValue(e => e.MinValueProperty) + 1);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.MaxValueProperty), expectedMaxValue), Times.Once);
            }

            public class TestClass
            {
                public int MinValueProperty { get; set; }
                public int MaxValueProperty { get; set; }
                public int Int32Function() => default(int);
            }
        }
    }
}
