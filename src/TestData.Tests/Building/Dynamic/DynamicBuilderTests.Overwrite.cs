﻿using FluentAssertions;
using TestData.Building.Standard;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderTests
    {
        public class Overwrite : DynamicBuilderTests
        {
            [Fact]
            public void NoOverwrites_ShouldReturnInstanceFromFactory()
            {
                // arrange
                var expectedInstance = new SomeClass();
                int originalPropertyValue = expectedInstance.ReadOnly;
                var sut = new DynamicBuilder<SomeClass>(
                    instanceFactory: StandardBuild.CreateInstanceFactoryFromDelegate(() => expectedInstance));

                // act
                var result = sut.Build();

                // assert
                result.Should().BeSameAs(expectedInstance);
                result.ReadOnly.Should().Be(originalPropertyValue);
            }

            [Fact]
            public void ShouldOverwriteReadOnlyProperty()
            {
                // arrange
                var sut = new DynamicBuilder<SomeClass>(
                    instanceFactory: StandardBuild.CreateInstanceFactoryFromDelegate(() => new SomeClass()));
                int expectedValue = 1;

                // act
                sut.Overwrite(nameof(SomeClass.ReadOnly), expectedValue);
                var result = sut.Build();

                // assert
                result.ReadOnly.Should().Be(expectedValue);
            }

            [Fact]
            public void ShouldOverwritePropertyWithPrivateSetter()
            {
                // arrange
                var sut = new DynamicBuilder<SomeClass>(
                    instanceFactory: StandardBuild.CreateInstanceFactoryFromDelegate(() => new SomeClass()));
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
                    instanceFactory: StandardBuild.CreateInstanceFactoryFromDelegate(() => new SomeClass()));
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
                    instanceFactory: StandardBuild.CreateInstanceFactoryFromDelegate(() => new SomeClass()));
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
                public SomeClass(int readOnly)
                {
                    ReadOnly = readOnly;
                }

                public int ReadOnly { get; }
                public int PublicGetPrivateSet { get; private set; }
                public int PublicGetProtectedSet { get; protected set; }
                public int PublicGetInternalSet { get; internal set; }
            }
        }
    }
}
