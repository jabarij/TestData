using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderExtensionsTests
    {
        public class WithBuilderDependentChild : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClassParent> builder = null;

                // act
                Action withBuilderDependentChild = () => DynamicBuilderExtensions.WithBuilderDependentChild(builder, e => e.Child, (parentBuilder, childBuilder) => childBuilder
                    .WithValue(c => c.ChildValueProperty, parentBuilder.GetOverwrittenValue(e => e.ParentValueProperty) + 1));

                // assert
                withBuilderDependentChild.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullPropertyFunc_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClassParent>>();

                // act
                Action withBuilderDependentChild = () => DynamicBuilderExtensions.WithBuilderDependentChild<TestClassParent, TestClassChild>(builderMock.Object, null, (parentBuilder, childBuilder) => childBuilder
                    .WithValue(c => c.ChildValueProperty, parentBuilder.GetOverwrittenValue(e => e.ParentValueProperty) + 1));

                // assert
                withBuilderDependentChild.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullBuildChildAction_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClassParent>>();

                // act
                Action withBuilderDependentChild = () => DynamicBuilderExtensions.WithBuilderDependentChild(builderMock.Object, p => p.Child, null);

                // assert
                withBuilderDependentChild.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClassParent>>();

                // act
                Action withBuilderDependentChild = () => DynamicBuilderExtensions.WithBuilderDependentChild(builderMock.Object, e => e.ChildFunction(), (parentBuilder, childBuilder) => childBuilder
                    .WithValue(c => c.ChildValueProperty, parentBuilder.GetOverwrittenValue(e => e.ParentValueProperty) + 1));

                // assert
                var exception = withBuilderDependentChild.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClassParent>>();
                var child = new TestClassChild();
                int parentValue = 1;
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClassParent.ParentValueProperty))).Returns(true);
                builderMock.Setup(e => e.GetOverwrittenValue(nameof(TestClassParent.ParentValueProperty))).Returns(parentValue);
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClassParent.Child))).Returns(true);
                builderMock.Setup(e => e.GetOverwrittenValue(nameof(TestClassParent.Child))).Returns(child);
                int expectedChildValue = parentValue + 1;

                // act
                var builder = DynamicBuilderExtensions.WithBuilderDependentChild(builderMock.Object, e => e.Child, (parentBuilder, childBuilder) => childBuilder
                    .WithValue(c => c.ChildValueProperty, parentBuilder.GetOverwrittenValue(e => e.ParentValueProperty) + 1));

                // assert
                builderMock.Verify(e => e.Overwrite(nameof(TestClassParent.Child), It.Is<TestClassChild>(c => c.ChildValueProperty == expectedChildValue)), Times.Once);
            }

            public class TestClassParent
            {
                public int ParentValueProperty { get; set; }
                public TestClassChild Child { get; set; }
                public TestClassChild ChildFunction() => default(TestClassChild);
            }

            public class TestClassChild
            {
                public int ChildValueProperty { get; set; }
            }
        }
    }
}
