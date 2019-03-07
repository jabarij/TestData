using FluentAssertions;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderExtensionsTests
    {
        public class WithChild : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestParentClass> builder = null;

                // act
                Action withChild = () => DynamicBuilderExtensions.WithChild(builder, p => p.Child, childBuilder => childBuilder.WithValue(c => c.Int32Property, 1));

                // assert
                withChild.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullPropertyFunc_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestParentClass> builder = new DynamicBuilder<TestParentClass>();

                // act
                Action withChild = () => DynamicBuilderExtensions.WithChild<TestParentClass, TestChildClass>(builder, null, childBuilder => childBuilder.WithValue(c => c.Int32Property, 1));

                // assert
                withChild.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullBuildChildAction_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestParentClass> builder = new DynamicBuilder<TestParentClass>();

                // act
                Action withChild = () => DynamicBuilderExtensions.WithChild(builder, p => p.Child, null);

                // assert
                withChild.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ShouldChangeOnlyOverwrittenPropertyInChildObject()
            {
                // arrange
                var parentObject = new TestParentClass(new TestChildClass(1, Guid.NewGuid().ToString()));
                var parentBuilder = Build.Dynamically(parentObject);
                string expectedStringValue = Guid.NewGuid().ToString();

                // act
                var resultParentObject = DynamicBuilderExtensions
                    .WithChild(parentBuilder, p => p.Child, childBuilder => childBuilder.WithValue(c => c.StringProperty, expectedStringValue))
                    .Build();

                // assert
                resultParentObject.Child.Should().BeEquivalentTo(parentObject.Child, opt => opt.Excluding(e => e.StringProperty));
                resultParentObject.Child.StringProperty.Should().Be(expectedStringValue);
            }

            [Fact]
            public void NullChild_ShouldCreateNewChildWithOverwrittenProperties()
            {
                // arrange
                var parentObject = new TestParentClass(null);
                var parentBuilder = Build.Dynamically(parentObject);
                string expectedStringValue = Guid.NewGuid().ToString();

                // act
                var resultParentObject = DynamicBuilderExtensions
                    .WithChild(parentBuilder, p => p.Child, childBuilder => childBuilder.WithValue(c => c.StringProperty, expectedStringValue))
                    .Build();

                // assert
                resultParentObject.Child.Should().NotBeNull();
                resultParentObject.Child.Int32Property.Should().Be(default(int));
                resultParentObject.Child.StringProperty.Should().Be(expectedStringValue);
            }

            public class TestParentClass
            {
                public TestParentClass(TestChildClass child)
                {
                    Child = child;
                }

                public TestChildClass Child { get; }
            }

            public class TestChildClass
            {
                public TestChildClass(int int32Property, string stringProperty)
                {
                    Int32Property = int32Property;
                    StringProperty = stringProperty;
                }

                public int Int32Property { get; }
                public string StringProperty { get; }
            }
        }
    }
}
