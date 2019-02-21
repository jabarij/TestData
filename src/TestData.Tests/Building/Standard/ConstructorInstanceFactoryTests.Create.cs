using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace TestData.Building.Standard
{
    partial class ConstructorInstanceFactoryTests
    {
        public class Create : ConstructorInstanceFactoryTests
        {
            [Fact]
            public void NullConstructorSelected_ShouldThrow()
            {
                // arrange
                var sut = new ConstructorInstanceFactory<SomeClass>(
                    constructorSelector: new DelegateConstructorSelector<SomeClass>(t => null));

                // act
                Action create = () => sut.Create(Enumerable.Empty<INamedPropertyOverwriter>());

                // assert
                var exception = create.Should().Throw<InvalidOperationException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.ConstructorNotFound.Code);
            }

            [Fact]
            public void NoOverwriterMatchingConstructorParameter_ShouldCreateWithDefaultValueForConstructorParameter()
            {
                // arrange
                var sut = new ConstructorInstanceFactory<SomeClass>(
                    constructorSelector: new DelegateConstructorSelector<SomeClass>(t => t.GetConstructor(new Type[] { typeof(int) })));
                var expected = new SomeClass(default(int));

                // act
                var result = sut.Create(Enumerable.Empty<INamedPropertyOverwriter>());

                // assert
                result.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void ShouldCreateWithOverwrittenValueForConstructorParameter()
            {
                // arrange
                var sut = new ConstructorInstanceFactory<SomeClass>(
                    constructorSelector: new DelegateConstructorSelector<SomeClass>(t => t.GetConstructor(new Type[] { typeof(int) })));
                int overwrittenValue = 1;
                var expected = new SomeClass(overwrittenValue);

                // act
                var result = sut.Create(new INamedPropertyOverwriter[] { new NamedPropertyOverwriter<int>("param", overwrittenValue) });

                // assert
                result.Should().BeEquivalentTo(expected);
            }

            class SomeClass
            {
                public SomeClass(int param) { Param = param; }

                public int Param { get; }
            }
        }
    }
}
