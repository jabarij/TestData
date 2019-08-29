using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderExtensionsTests
    {
        public class WithNull : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withNull = () => DynamicBuilderExtensions.WithNull(builder, e => e.StringProperty);

                // assert
                withNull.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withNull = () => DynamicBuilderExtensions.WithNull<TestClass, string>(builderMock.Object, null);

                // assert
                withNull.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withNull = () => DynamicBuilderExtensions.WithNull(builderMock.Object, e => e.StringFunction());

                // assert
                var exception = withNull.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ClassProperty_ShouldOverwriteWithNull()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                var builder = DynamicBuilderExtensions.WithNull(builderMock.Object, e => e.StringProperty);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.StringProperty), (string)null), Times.Once);
            }

            [Fact]
            public void NullableProperty_ShouldOverwriteWithNull()
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
