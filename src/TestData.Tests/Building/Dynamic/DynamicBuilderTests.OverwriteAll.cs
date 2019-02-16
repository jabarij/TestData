using FluentAssertions;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderTests
    {
        public class OverwriteAll : DynamicBuilderTests
        {
            [Fact]
            public void NullTemplate_ShouldThrow()
            {
                // arrange
                var sut = new DynamicBuilder<SomeClass>();

                // act
                Action overwriteAll = () => sut.OverwriteAll(null);

                // assert
                overwriteAll.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ShouldOverwriteAllPropertiesWithValuesFromTemplate()
            {
                // arrange
                var sut = new DynamicBuilder<SomeClass>();
                sut.Overwrite(nameof(SomeClass.Int32Property), 1);
                var template = new SomeClass
                {
                    Int32Property = 2
                };

                // act
                sut.OverwriteAll(template);
                var result = sut.Build();

                // assert
                result.Should().BeEquivalentTo(template);
            }

            class SomeClass
            {
                public int Int32Property { get; set; }
            }
        }
    }
}
