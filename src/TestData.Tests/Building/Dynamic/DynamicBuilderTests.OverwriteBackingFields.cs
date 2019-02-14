using FluentAssertions;
using System.Text.RegularExpressions;
using TestData.Building.Standard;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderTests
    {
        public class OverwriteBackingFields : DynamicBuilderTests
        {
            [Fact]
            public void ShouldOverwriteReadOnlyAutoProperty()
            {
                // arrange
                var sut = new DynamicBuilder<SomeClass>(
                    instanceFactory: StandardBuild.CreateInstanceFactoryFromDelegate(() => new SomeClass()));
                int expectedValue = 1;

                // act
                sut.Overwrite(nameof(SomeClass.ReadOnlyAutoProperty), expectedValue);
                var result = sut.Build();

                // assert
                result.ReadOnlyAutoProperty.Should().Be(expectedValue);
            }

            [Fact]
            public void ShouldOverwriteReadOnlyPropertyWithExplicitBackingField()
            {
                // arrange
                var sut = new DynamicBuilder<SomeClass>(
                    instanceFactory: StandardBuild.CreateInstanceFactoryFromDelegate(() => new SomeClass()),
                    propertyBackingFieldSelector: SelectBackingField.FromPropertyNameByRegex(@"^([A-Z])", m => $"_{m.Value.ToLower()}"));
                int expectedValue = 1;

                // act
                sut.Overwrite(nameof(SomeClass.ReadOnlyPropertyWithExplicitBackingField), expectedValue);
                var result = sut.Build();

                // assert
                result.ReadOnlyPropertyWithExplicitBackingField.Should().Be(expectedValue);
            }

            class SomeClass
            {
                public SomeClass() { }
                public SomeClass(int readOnlyAutoProperty, int readOnlyPropertyWithExplicitBackingField)
                {
                    ReadOnlyAutoProperty = readOnlyAutoProperty;
                    _readOnlyPropertyWithExplicitBackingField = readOnlyPropertyWithExplicitBackingField;
                }

                public int ReadOnlyAutoProperty { get; }

                private readonly int _readOnlyPropertyWithExplicitBackingField;
                public int ReadOnlyPropertyWithExplicitBackingField => _readOnlyPropertyWithExplicitBackingField;
            }
        }
    }
}
