using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderExtensionsTests
    {
        public class WithDependentValue : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withDependentValue = () => DynamicBuilderExtensions.WithDependentValue(builder, e => e.MinValueProperty, db => 1);

                // assert
                withDependentValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withDependentValue = () => DynamicBuilderExtensions.WithDependentValue(builderMock.Object, null, db => 1);

                // assert
                withDependentValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullGetBuilder_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withDependentValue = () => DynamicBuilderExtensions.WithDependentValue(builderMock.Object, e => e.MinValueProperty, null);

                // assert
                withDependentValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withDependentValue = () => DynamicBuilderExtensions.WithDependentValue(builderMock.Object, e => e.Int32Function(), db => 1);

                // assert
                var exception = withDependentValue.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                int minValue = 1;
                builderMock.Setup(e => e.Build()).Returns(new TestClass { MinValueProperty = minValue });
                int expectedMaxValue = minValue + 1;

                // act
                var builder = DynamicBuilderExtensions.WithDependentValue(builderMock.Object, e => e.MaxValueProperty, lender => lender.MinValueProperty + 1);

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
