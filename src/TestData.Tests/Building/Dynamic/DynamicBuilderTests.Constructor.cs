using FluentAssertions;
using TestData.Building.Standard;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderTests
    {
        public class Constructor : DynamicBuilderTests
        {
            [Fact]
            public void NoParams_ShouldCreateWithDefaultValues()
            {
                // arrange
                DynamicBuilder<SomeClass> sut;

                // act
                sut = new DynamicBuilder<SomeClass>();

                // assert
                ShouldHaveAllDependencies(sut);
            }

            [Theory]
            [InlineData(ConstructorSelection.LeastParameters)]
            [InlineData(ConstructorSelection.MostParameters)]
            public void ConstructorSelection_ShouldCreateWithGivenValue(ConstructorSelection constructorSelection)
            {
                // arrange
                DynamicBuilder<SomeClass> sut;

                // act
                sut = new DynamicBuilder<SomeClass>(constructorSelection);

                // assert
                sut.InstanceFactory.Should().NotBeNull();
                sut.InstanceFactory.Should().BeOfType<InstanceFactory<SomeClass>>();
                var instanceFactory = (InstanceFactory<SomeClass>)sut.InstanceFactory;
                instanceFactory.ConstructorSelector.Should().BeOfType<ConstructorSelector<SomeClass>>();
                var constructorSelector = (ConstructorSelector<SomeClass>)instanceFactory.ConstructorSelector;
                constructorSelector.ConstructorSelection.Should().Be(constructorSelection);
                ShouldHaveAllDependencies(sut);
            }

            [Fact]
            public void InstanceFactory_ShouldCreateWithGivenValue()
            {
                // arrange
                DynamicBuilder<SomeClass> sut;
                var instanceFactory = new InstanceFactory<SomeClass>();

                // act
                sut = new DynamicBuilder<SomeClass>(instanceFactory);

                // assert
                sut.InstanceFactory.Should().BeSameAs(instanceFactory);
                ShouldHaveAllDependencies(sut);
            }

            [Fact]
            public void PropertyBackingFieldSelector_ShouldCreateWithGivenValue()
            {
                // arrange
                DynamicBuilder<SomeClass> sut;
                var propertyBackingFieldSelector = new ReadOnlyPropertyBackingFieldSelector();

                // act
                sut = new DynamicBuilder<SomeClass>(propertyBackingFieldSelector);

                // assert
                sut.PropertyBackingFieldSelector.Should().BeSameAs(propertyBackingFieldSelector);
                ShouldHaveAllDependencies(sut);
            }

            [Fact]
            public void InstanceFactory_And_PropertyBackingFieldSelector_ShouldCreateWithGivenValues()
            {
                // arrange
                DynamicBuilder<SomeClass> sut;
                var instanceFactory = new InstanceFactory<SomeClass>();
                var propertyBackingFieldSelector = new ReadOnlyPropertyBackingFieldSelector();

                // act
                sut = new DynamicBuilder<SomeClass>(instanceFactory, propertyBackingFieldSelector);

                // assert
                sut.InstanceFactory.Should().BeSameAs(instanceFactory);
                sut.PropertyBackingFieldSelector.Should().BeSameAs(propertyBackingFieldSelector);
                ShouldHaveAllDependencies(sut);
            }

            private void ShouldHaveAllDependencies(DynamicBuilder<SomeClass> sut)
            {
                sut.InstanceFactory.Should().NotBeNull();
                sut.PropertyBackingFieldSelector.Should().NotBeNull();
            }

            class SomeClass
            {

            }
        }
    }
}
