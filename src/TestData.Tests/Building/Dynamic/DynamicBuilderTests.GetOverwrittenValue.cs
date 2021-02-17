using FluentAssertions;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderTests
    {
        public class GetOverwrittenValue : DynamicBuilderTests
        {
            public class ReferenceType : GetOverwrittenValue
            {
                [Theory]
                [InlineData(null)]
                [InlineData("")]
                [InlineData("   ")]
                public void InvalidName_ShouldThrow(string invalidName)
                {
                    // arrange
                    var sut = new DynamicBuilder<SomeClass>();

                    // act
                    Action getOverwrittenValue = () => sut.GetOverwrittenValue(invalidName);

                    // assert
                    getOverwrittenValue.Should().Throw<ArgumentNullException>();
                }

                [Fact]
                public void UnknownName_ShouldThrow()
                {
                    // arrange
                    var sut = new DynamicBuilder<SomeClass>();

                    // act
                    Action getOverwrittenValue = () => sut.GetOverwrittenValue("UnknownProperty");

                    // assert
                    getOverwrittenValue.Should().Throw<InvalidOperationException>();
                }

                [Fact]
                public void NotOverwritten_ShouldReturnDefaultValueOfPropertyType()
                {
                    // arrange
                    var sut = new DynamicBuilder<SomeClass>();

                    // act
                    object result = sut.GetOverwrittenValue(nameof(SomeClass.Int32Property));

                    // assert
                    result.Should().Be(default(int));
                }

                [Fact]
                public void Overwritten_ShouldReturnOverwrittenValue()
                {
                    // arrange
                    var sut = new DynamicBuilder<SomeClass>();
                    int expected = 1;
                    sut.Overwrite(nameof(SomeClass.Int32Property), expected);

                    // act
                    object result = sut.GetOverwrittenValue(nameof(SomeClass.Int32Property));

                    // assert
                    result.Should().Be(expected);
                }

                class SomeClass
                {
                    public int Int32Property { get; set; }
                    public string StringProperty { get; set; }
                }
            }

            public class ValueType : GetOverwrittenValue
            {
                [Theory]
                [InlineData(null)]
                [InlineData("")]
                [InlineData("   ")]
                public void InvalidName_ShouldThrow(string invalidName)
                {
                    // arrange
                    var sut = new DynamicBuilder<SomeStruct>();

                    // act
                    Action getOverwrittenValue = () => sut.GetOverwrittenValue(invalidName);

                    // assert
                    getOverwrittenValue.Should().Throw<ArgumentNullException>();
                }

                [Fact]
                public void UnknownName_ShouldThrow()
                {
                    // arrange
                    var sut = new DynamicBuilder<SomeStruct>();

                    // act
                    Action getOverwrittenValue = () => sut.GetOverwrittenValue("UnknownProperty");

                    // assert
                    getOverwrittenValue.Should().Throw<InvalidOperationException>();
                }

                [Fact]
                public void NotOverwritten_ShouldReturnDefaultValueOfPropertyType()
                {
                    // arrange
                    var sut = new DynamicBuilder<SomeStruct>();

                    // act
                    object result = sut.GetOverwrittenValue(nameof(SomeStruct.Int32Property));

                    // assert
                    result.Should().Be(default(int));
                }

                [Fact]
                public void Overwritten_ShouldReturnOverwrittenValue()
                {
                    // arrange
                    var sut = new DynamicBuilder<SomeStruct>();
                    int expected = 1;
                    sut.Overwrite(nameof(SomeStruct.Int32Property), expected);

                    // act
                    object result = sut.GetOverwrittenValue(nameof(SomeStruct.Int32Property));

                    // assert
                    result.Should().Be(expected);
                }

                struct SomeStruct
                {
                    public int Int32Property { get; set; }
                    public string StringProperty { get; set; }
                }
            }
        }
    }
}
