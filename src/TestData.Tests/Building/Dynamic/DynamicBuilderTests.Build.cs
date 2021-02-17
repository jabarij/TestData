using FluentAssertions;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderTests
    {
        public class Build : DynamicBuilderTests
        {
            public class ReferenceType : Build
            {
                [Fact]
                public void ShouldBuildObjectWithOverwrittenProperties()
                {
                    // arrange
                    var template = new SomeClass(
                        intProperty: 1,
                        stringProperty: "some text");
                    int expectedIntProperty = template.IntProperty;
                    string expectedStringProperty = "other text";

                    var sut = new DynamicBuilder<SomeClass>();
                    sut.OverwriteWithTemplate(template);
                    sut.Overwrite(nameof(SomeClass.StringProperty), expectedStringProperty);

                    // act
                    var result = sut.Build();

                    // assert
                    result.IntProperty.Should().Be(expectedIntProperty);
                    result.StringProperty.Should().Be(expectedStringProperty);
                }

                class SomeClass
                {
                    public SomeClass(int intProperty, string stringProperty)
                    {
                        IntProperty = intProperty;
                        StringProperty = stringProperty;
                    }

                    public object[] Values =>
                        new object[] { IntProperty, StringProperty };

                    public int IntProperty { get; }
                    public string StringProperty { get; }
                }
            }

            public class ValueType : Build
            {
                [Fact]
                public void ShouldBuildObjectWithOverwrittenProperties()
                {
                    // arrange
                    var template = new SomeStruct(
                        intProperty: 1,
                        stringProperty: "some text");
                    int expectedIntProperty = template.IntProperty;
                    string expectedStringProperty = "other text";

                    var sut = new DynamicBuilder<SomeStruct>();
                    sut.OverwriteWithTemplate(template);
                    sut.Overwrite(nameof(SomeStruct.StringProperty), expectedStringProperty);

                    // act
                    var result = sut.Build();

                    // assert
                    result.IntProperty.Should().Be(expectedIntProperty);
                    result.StringProperty.Should().Be(expectedStringProperty);
                }

                struct SomeStruct
                {
                    public SomeStruct(int intProperty, string stringProperty)
                    {
                        IntProperty = intProperty;
                        StringProperty = stringProperty;
                    }

                    public object[] Values =>
                        new object[] { IntProperty, StringProperty };

                    public int IntProperty { get; }
                    public string StringProperty { get; }
                }
            }
        }
    }
}
