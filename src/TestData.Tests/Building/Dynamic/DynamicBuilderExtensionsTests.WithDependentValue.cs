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
                Action withValue = () => DynamicBuilderExtensions.WithDependentValue(builder, e => e.Int32Property, db => 1);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithDependentValue(builderMock.Object, null, db => 1);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullGetBuilder_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithDependentValue(builderMock.Object, e => e.Int32Property, null);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithDependentValue(builderMock.Object, e => e.Int32Function(), db => 1);

                // assert
                var exception = withValue.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                int expectedValue = 1;

                // act
                var builder = DynamicBuilderExtensions.WithDependentValue(builderMock.Object, e => e.Int32Property, db => expectedValue);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.Int32Property), expectedValue), Times.Once);
            }

            public class TestClass
            {
                public int Int32Property { get; set; }
                public int Int32Function() => default(int);
            }
        }
    }
}
