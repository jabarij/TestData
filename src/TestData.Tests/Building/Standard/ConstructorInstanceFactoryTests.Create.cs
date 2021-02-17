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
            public class ReferenceType : Create
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
                        constructorSelector: new DelegateConstructorSelector<SomeClass>(t => t.GetConstructor(new Type[] { typeof(int), typeof(string) })));
                    var expected = new SomeClass(
                        intProperty: default(int),
                        stringProperty: default(string));

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
                        constructorSelector: new DelegateConstructorSelector<SomeClass>(t => t.GetConstructor(new Type[] { typeof(int), typeof(string) })));
                    int expectedIntProperty = 1;
                    string expectedStringProperty = "test";
                    var expected = new SomeClass(
                        intProperty: expectedIntProperty,
                        stringProperty: expectedStringProperty);

                    // act
                    var result = sut.Create(new INamedPropertyOverwriter[]
                    {
                        new NamedPropertyOverwriter<int>("intProperty", expectedIntProperty),
                        new NamedPropertyOverwriter<string>("stringProperty", expectedStringProperty)
                    });

                    // assert
                    result.Should().BeEquivalentTo(expected);
                }

                class SomeClass
                {
                    public SomeClass(int intProperty, string stringProperty)
                    {
                        IntProperty = intProperty;
                        StringProperty = stringProperty;
                    }

                    public int IntProperty { get; }
                    public string StringProperty { get; }
                }
            }

            public class ValueType : Create
            {
                [Fact]
                public void NullConstructorSelected_ShouldThrow()
                {
                    // arrange
                    var sut = new ConstructorInstanceFactory<SomeStruct>(
                        constructorSelector: new DelegateConstructorSelector<SomeStruct>(t => null));

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
                    var sut = new ConstructorInstanceFactory<SomeStruct>(
                        constructorSelector: new DelegateConstructorSelector<SomeStruct>(t => t.GetConstructor(new Type[] { typeof(int), typeof(string) })));
                    var expected = new SomeStruct(
                        intProperty: default(int),
                        stringProperty: default(string));

                    // act
                    var result = sut.Create(Enumerable.Empty<INamedPropertyOverwriter>());

                    // assert
                    result.Should().BeEquivalentTo(expected);
                }

                [Fact]
                public void ShouldCreateWithOverwrittenValueForConstructorParameter()
                {
                    // arrange
                    var sut = new ConstructorInstanceFactory<SomeStruct>(
                        constructorSelector: new DelegateConstructorSelector<SomeStruct>(t => t.GetConstructor(new Type[] { typeof(int), typeof(string) })));
                    int expectedIntProperty = 1;
                    string expectedStringProperty = "test";
                    var expected = new SomeStruct(
                        intProperty: expectedIntProperty,
                        stringProperty: expectedStringProperty);

                    // act
                    var result = sut.Create(new INamedPropertyOverwriter[]
                    {
                        new NamedPropertyOverwriter<int>("intProperty", expectedIntProperty),
                        new NamedPropertyOverwriter<string>("stringProperty", expectedStringProperty)
                    });

                    // assert
                    result.Should().BeEquivalentTo(expected);
                }

                struct SomeStruct
                {
                    public SomeStruct(int intProperty, string stringProperty)
                    {
                        IntProperty = intProperty;
                        StringProperty = stringProperty;
                    }

                    public int IntProperty { get; }
                    public string StringProperty { get; }
                }
            }
        }
    }
}
