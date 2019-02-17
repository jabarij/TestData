using FluentAssertions;
using System;
using Xunit;

namespace TestData.Building.Standard
{
    partial class ConstructorInstanceFactoryTests
    {
        public class Constructor : ConstructorInstanceFactoryTests
        {
            [Fact]
            public void NullConstructorSelector_ShouldThrow()
            {
                // arrange
                // act
                Action create = () => new ConstructorInstanceFactory<SomeClass>(
                    constructorSelector: null);

                // assert
                create.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ConstructorSelector_ShouldCreateWithGivenObject()
            {
                // arrange
                var expected = new ConstructorSelector<SomeClass>();

                // act
                var sut = new ConstructorInstanceFactory<SomeClass>(
                    constructorSelector: expected);

                // assert
                sut.ConstructorSelector.Should().BeSameAs(expected);
            }

            class SomeClass
            {
            }
        }
    }
}
