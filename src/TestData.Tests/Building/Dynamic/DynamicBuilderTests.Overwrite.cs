using Moq;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderTests
    {
        public class Overwrite : DynamicBuilderTests
        {
            [Theory]
            [InlineData(nameof(TestClass.PublicGetPublicSet), 1)]
            [InlineData(nameof(TestClass.PublicGetInternalSet), 1)]
            [InlineData(nameof(TestClass.PublicGetProtectedSet), 1)]
            [InlineData(nameof(TestClass.PublicGetPrivateSet), 1)]
            [InlineData(nameof(TestClass.PublicReadOnlyAutoProperty), 1)]
            [InlineData(nameof(TestClass.PublicGetOfPrivateField), 1)]
            [InlineData(nameof(TestClass.InternalGetInternalSet), 1)]
            [InlineData(nameof(TestClass.InternalGetPrivateSet), 1)]
            [InlineData(nameof(TestClass.ProtectedInternalGetProtectedInternalSet), 1)]
            [InlineData(nameof(TestClass.ProtectedInternalGetInternalSet), 1)]
            [InlineData(nameof(TestClass.ProtectedInternalGetPrivateSet), 1)]
            public void ShouldCallPropertySetterForGivenProperty(string propertyName, object expectedValue)
            {
                // arrange
                var expectedInstance = new TestClass();
                var instanceFactoryMock = new Mock<IInstanceFactory<TestClass>>();
                instanceFactoryMock
                    .Setup(e => e.Create(It.IsAny<IEnumerable<INamedPropertyOverwriter>>()))
                    .Returns(expectedInstance);
                var propertySetterMock = new Mock<IPropertySetter>();

                var sut = new DynamicBuilder<TestClass>(
                    instanceFactory: instanceFactoryMock.Object,
                    propertySetter: propertySetterMock.Object);
                var expectedProperty = expectedInstance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                // act
                sut.Overwrite(propertyName, expectedValue);
                sut.Build();

                // assert
                propertySetterMock
                    .Verify(e => e.SetProperty(expectedInstance, expectedProperty, expectedValue), Times.Once());
            }

            public class TestClass
            {

                public int PublicGetPublicSet { get; set; }
                public int PublicGetInternalSet { get; internal set; }
                public int PublicGetProtectedSet { get; protected set; }
                public int PublicGetPrivateSet { get; private set; }
                public int PublicReadOnlyAutoProperty { get; }
                private readonly int _publicGetOfPrivateField;
                public int PublicGetOfPrivateField => _publicGetOfPrivateField;

                internal int InternalGetInternalSet { get; set; }
                internal int InternalGetPrivateSet { get; private set; }

                protected internal int ProtectedInternalGetProtectedInternalSet { get; set; }
                protected internal int ProtectedInternalGetInternalSet { get; internal set; }
                protected internal int ProtectedInternalGetPrivateSet { get; private set; }

                protected int ProtectedGetProtectedSet { get; set; }
                protected int ProtectedGetPrivateSet { get; private set; }

                private int PrivateGetPrivateSet { get; set; }
            }
        }
    }
}
