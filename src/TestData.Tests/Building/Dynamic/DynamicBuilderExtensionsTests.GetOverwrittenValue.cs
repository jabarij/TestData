using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderExtensionsTests
    {
        public class GetOverwrittenValue : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClass> builder = null;

                // act
                Action getOverwrittenValue = () => DynamicBuilderExtensions.GetOverwrittenValue(builder, e => e.StringProperty);

                // assert
                getOverwrittenValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullExpression_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action getOverwrittenValue = () => DynamicBuilderExtensions.GetOverwrittenValue<TestClass, string>(builderMock.Object, null);

                // assert
                getOverwrittenValue.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();

                // act
                Action getOverwrittenValue = () => DynamicBuilderExtensions.GetOverwrittenValue(builderMock.Object, e => e.StringFunction());

                // assert
                var exception = getOverwrittenValue.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ClassProperty_ShouldOverwriteWithNull()
            {
                // arrange
                string expectedResult = Guid.NewGuid().ToString();
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock
                    .Setup(e => e.IsOverwritten(nameof(TestClass.StringProperty)))
                    .Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue(nameof(TestClass.StringProperty)))
                    .Returns(expectedResult);

                // act
                string result = DynamicBuilderExtensions.GetOverwrittenValue(builderMock.Object, e => e.StringProperty);

                // assert
                result.Should().Be(expectedResult);
            }

            [Fact]
            public void NullableProperty_ShouldOverwriteWithNull()
            {
                // arrange
                int? expectedResult = 1;
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock
                    .Setup(e => e.IsOverwritten(nameof(TestClass.NullableInt32Property)))
                    .Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue(nameof(TestClass.NullableInt32Property)))
                    .Returns((int?)1);

                // act
                int? result = DynamicBuilderExtensions.GetOverwrittenValue(builderMock.Object, e => e.NullableInt32Property);

                // assert
                result.Should().Be(expectedResult);
            }

            [Fact]
            public void StructProperty_ShouldOverwriteGetOverwrittenValueValue()
            {
                // arrange
                Guid expectedResult = Guid.NewGuid();
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock
                    .Setup(e => e.IsOverwritten(nameof(TestClass.GuidProperty)))
                    .Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue(nameof(TestClass.GuidProperty)))
                    .Returns(expectedResult);

                // act
                var result = DynamicBuilderExtensions.GetOverwrittenValue(builderMock.Object, e => e.GuidProperty);

                // assert
                result.Should().Be(expectedResult);
            }

            [Fact]
            public void NullablePropertyWithCasting_ShouldOverwriteWithNull()
            {
                // arrange
                int? expectedResult = 1;
                var builderMock = new Mock<IDynamicBuilder<TestClass>>();
                builderMock
                    .Setup(e => e.IsOverwritten(nameof(TestClass.NullableInt32Property)))
                    .Returns(true);
                builderMock
                    .Setup(e => e.GetOverwrittenValue(nameof(TestClass.NullableInt32Property)))
                    .Returns(1);

                // act
                int? result = DynamicBuilderExtensions.GetOverwrittenValue(builderMock.Object, e => (int)e.NullableInt32Property);

                // assert
                result.Should().Be(expectedResult);
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
