using FluentAssertions;
using TestData.Building.Standard;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderTests
    {
        public class OverwriteBackingFields : DynamicBuilderTests
        {
            [Theory]
            [InlineData(nameof(SomeClass.ReadOnlyAutoProperty), 1)]
            [InlineData(nameof(SomeClass.ReadOnlyPropertyWithExplicitBackingField), 1)]
            public void ShouldSetPropertyUsingPropertySetter(string propertyName, object expectedValue)
            {
                // arrange
                dynamic setPropertyArgs = null;
                var instance = new SomeClass();
                var sut = new DynamicBuilder<SomeClass>(
                    instanceFactory: new DelegateInstanceFactory<SomeClass>(() => instance),
                    propertySetter: new DelegatePropertySetter((ownr, prop, val) => setPropertyArgs = new { Owner = ownr, Property = prop, Value = val }));

                // act
                sut.Overwrite(propertyName, expectedValue);
                var result = sut.Build();

                // assert
                ((object)setPropertyArgs).Should().NotBeNull();
                ((object)setPropertyArgs.Owner).Should().BeSameAs(instance);
                ((object)setPropertyArgs.Property.Name).Should().Be(propertyName);
                ((object)setPropertyArgs.Value).Should().Be(expectedValue);
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
