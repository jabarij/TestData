using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderExtensionsTests
    {
        public class WithDependentChild : DynamicBuilderExtensionsTests
        {
            [Fact]
            public void NullBuilder_ShouldThrow()
            {
                // arrange
                IDynamicBuilder<TestClassParent> builder = null;

                // act
                Action withDependentChild = () => DynamicBuilderExtensions.WithDependentChild(builder, e => e.Child, (parentObj, childBuilder) => childBuilder
                    .WithValue(c => c.ChildValueProperty, parentObj.ParentValueProperty + 1));

                // assert
                withDependentChild.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullPropertyFunc_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClassParent>>();

                // act
                Action withDependentChild = () => DynamicBuilderExtensions.WithDependentChild<TestClassParent, TestClassChild>(builderMock.Object, null, (parentObj, childBuilder) => childBuilder
                    .WithValue(c => c.ChildValueProperty, parentObj.ParentValueProperty + 1));

                // assert
                withDependentChild.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void NullBuildChildAction_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClassParent>>();

                // act
                Action withDependentChild = () => DynamicBuilderExtensions.WithDependentChild(builderMock.Object, p => p.Child, null);

                // assert
                withDependentChild.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ExpressionIsMethodCall_ShouldThrow()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClassParent>>();

                // act
                Action withDependentChild = () => DynamicBuilderExtensions.WithDependentChild(builderMock.Object, e => e.ChildFunction(), (parentObj, childBuilder) => childBuilder
                    .WithValue(c => c.ChildValueProperty, parentObj.ParentValueProperty + 1));

                // assert
                var exception = withDependentChild.Should().Throw<ArgumentException>().And;
                exception.Data[Errors.ErrorCodeExceptionDataKey].Should().Be(Errors.OnlyMemberAccessExpressionAreAllowed.Code);
            }

            [Fact]
            public void ShouldOverwritePropertyByExpression()
            {
                // arrange
                var builderMock = new Mock<IDynamicBuilder<TestClassParent>>();
                var child = new TestClassChild();
                int parentValue = 1;
                builderMock.Setup(e => e.Build()).Returns(new TestClassParent { ParentValueProperty = parentValue });
                builderMock.Setup(e => e.IsOverwritten(nameof(TestClassParent.Child))).Returns(true);
                builderMock.Setup(e => e.GetOverwrittenValue(nameof(TestClassParent.Child))).Returns(child);
                int expectedChildValue = parentValue + 1;

                // act
                var builder = DynamicBuilderExtensions.WithDependentChild(builderMock.Object, e => e.Child, (parentObj, childBuilder) => childBuilder
                    .WithValue(c => c.ChildValueProperty, parentObj.ParentValueProperty + 1));

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
