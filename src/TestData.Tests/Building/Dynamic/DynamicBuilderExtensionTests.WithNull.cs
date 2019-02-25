using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderExtensionTests
    {
        public class WithNull : DynamicBuilderExtensionTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withValue = () => DynamicBuilderExtensions.WithNull(builder, e => e.StringProperty);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithNull<TestClass, string>(builderMock.Object, null);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithNull(builderMock.Object, e => e.StringFunction());

                // assert
                var exception = withValue.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ClassProperty_ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                var builder = DynamicBuilderExtensions.WithNull(builderMock.Object, e => e.StringProperty);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.StringProperty), (string)null), Times.Once);
            }

            [Fact]
            public void NullableProperty_ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                var builder = DynamicBuilderExtensions.WithNull(builderMock.Object, e => e.NullableInt32Property);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.NullableInt32Property), (int?)null), Times.Once);
            }

            public class TestClass
            {
                public string StringProperty { get; set; }
                public string StringFunction() => null;
                public int? NullableInt32Property { get; set; }

            }
        }
    }
}
