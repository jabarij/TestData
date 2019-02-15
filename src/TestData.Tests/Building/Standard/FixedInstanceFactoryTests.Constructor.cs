using FluentAssertions;
using System;
using Xunit;

namespace TestData.Building.Standard
{
    partial class FixedInstanceFactoryTests
    {
        public class Constructor : FixedInstanceFactoryTests
        {
            [Fact]
            public void NullInstance_ShouldThrow()
            {
                // arrange
                // act
                Action create = () => new FixedInstanceFactory<SomeClass>(null);

                // assert
                create.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void Instance_ShouldCreateWithGivenObject()
            {
                // arrange
                var expected = new SomeClass();

                // act
                var sut = new FixedInstanceFactory<SomeClass>(expected);

                // assert
                sut.Instance.Should().BeSameAs(expected);
            }

            class SomeClass
            {
            }
        }
    }
}
