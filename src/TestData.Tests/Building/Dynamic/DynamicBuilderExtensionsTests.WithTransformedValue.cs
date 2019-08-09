using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderExtensionsTests
    {
        public class WithTransformedValue : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action withValue = () => DynamicBuilderExtensions.WithTransformedValue(builder, e => e.Int32Property, val => val);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithTransformedValue<TestClass, int>(builderMock.Object, null, val => val);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullTransformation_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithTransformedValue(builderMock.Object, e => e.Int32Property, null);

                // assert
                withValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action withValue = () => DynamicBuilderExtensions.WithTransformedValue(builderMock.Object, e => e.Int32Function(), val => val);

                // assert
                var exception = withValue.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ShouldOverwritePropertyWithTransformedValueByExpression()
            {
                // arrange
                int originalValue = 1;
                Func<int, int> transformation = v => v + 1;
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock
                    .Setup(e => e.IsOverwritten(nameof(TestClass.Int32Property)))
                    .Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue<int>(nameof(TestClass.Int32Property)))
                    .Returns(originalValue);

                // act
                var builder = DynamicBuilderExtensions.WithTransformedValue(builderMock.Object, e => e.Int32Property, transformation);

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClass.Int32Property), transformation(originalValue)), Times.Once);
            }

            public class TestClass
            {
                public int Int32Property { get; set; }
                public int Int32Function() => default(int);
            }
        }
    }
}
