using FluentAssertions;
using TestData.Building.Standard;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderTests
    {
        public class OverwriteProperties : DynamicBuilderTests
        {
            [Fact]
            public void NoOverwrites_ShouldReturnInstanceFromFactory()
            {
                // arrange
                var expectedInstance = new SomeClass();
                var sut = new DynamicBuilder<SomeClass>(
                    instanceFactory: new DelegateInstanceFactory<SomeClass>(() => expectedInstance));

                // act
                var result = sut.Build();

                // assert
                result.Should().BeSameAs(expectedInstance);
            }

            [Fact]
            public void ShouldOverwritePropertyWithPrivateSetter()
            {
                // arrange
                var sut = new DynamicBuilder<SomeClass>(
                    instanceFactory: new DelegateInstanceFactory<SomeClass>(() => new SomeClass()));
                int expectedValue = 1;

                // act
                sut.Overwrite(nameof(SomeClass.PublicGetPrivateSet), expectedValue);
                var result = sut.Build();

                // assert
                result.PublicGetPrivateSet.Should().Be(expectedValue);
            }

            [Fact]
            public void ShouldOverwritePropertyWithProtectedSetter()
            {
                // arrange
                var sut = new DynamicBuilder<SomeClass>(
                    instanceFactory: new DelegateInstanceFactory<SomeClass>(() => new SomeClass()));
                int expectedValue = 1;

                // act
                sut.Overwrite(nameof(SomeClass.PublicGetProtectedSet), expectedValue);
                var result = sut.Build();

                // assert
                result.PublicGetProtectedSet.Should().Be(expectedValue);
            }

            [Fact]
            public void ShouldOverwritePropertyWithInternalSetter()
            {
                // arrange
                var sut = new DynamicBuilder<SomeClass>(
                    instanceFactory: new DelegateInstanceFactory<SomeClass>(() => new SomeClass()));
                int expectedValue = 1;

                // act
                sut.Overwrite(nameof(SomeClass.PublicGetInternalSet), expectedValue);
                var result = sut.Build();

                // assert
                result.PublicGetInternalSet.Should().Be(expectedValue);
            }

            class SomeClass
            {
                public SomeClass() { }
                
                public int PublicGetPrivateSet { get; private set; }
                public int PublicGetProtectedSet { get; protected set; }
                public int PublicGetInternalSet { get; internal set; }
            }
        }
    }
}
