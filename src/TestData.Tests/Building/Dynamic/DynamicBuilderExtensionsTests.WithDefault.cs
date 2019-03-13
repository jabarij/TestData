using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderExtensionsTests
    {
        public class WithDefault : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withDefault = () => DynamicBuilderExtensions.WithDefault(builder, e => e.StringProperty);

                // assert
                withDefault.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withDefault = () => DynamicBuilderExtensions.WithDefault<TestClass, string>(builderMock.Object, null);

                // assert
                withDefault.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withDefault = () => DynamicBuilderExtensions.WithDefault(builderMock.Object, e => e.StringFunction());

                // assert
                var exception = withDefault.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ClassProperty_ShouldOverwriteWithNull()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                var builder = DynamicBuilderExtensions.WithDefault(builderMock.Object, e => e.StringProperty);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.StringProperty), (string)null), Times.Once);
            }

            [Fact]
            public void NullableProperty_ShouldOverwriteWithNull()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                var builder = DynamicBuilderExtensions.WithDefault(builderMock.Object, e => e.NullableInt32Property);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.NullableInt32Property), (int?)null), Times.Once);
            }

            [Fact]
            public void StructProperty_ShouldOverwriteWithDefaultValue()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                var builder = DynamicBuilderExtensions.WithDefault(builderMock.Object, e => e.GuidProperty);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.GuidProperty), Guid.Empty), Times.Once);
            }

            public class TestClass
            {
                public string StringProperty { get; set; }
                public string StringFunction() => null;
                public int? NullableInt32Property { get; set; }
                public Guid GuidProperty { get; set; }
            }
        }
    }
}
