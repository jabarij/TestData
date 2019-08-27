using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace TestData.Building.Dynamic
{
    partial class DynamicBuilderTests
    {
        public class OverwriteWithTemplate_TemplateOfDifferentTypeThanTarget : DynamicBuilderTests
        {
            [Fact]
            public void NullTemplate_ShouldThrow()
            {
                // arrange
                var sut = new DynamicBuilder<TestClass>();

                // act
                Action overwriteAll = () => sut.OverwriteWithTemplate(null);

                // assert
                overwriteAll.Should().Throw<ArgumentNullException>();
            }

            [Fact]
            public void ShouldCallPropertySetterForAllExpectedProperties()
            {
                // arrange
                var propertySetterMock = new Mock<IPropertySetter>();
                var sut = new DynamicBuilder<TestClass>(
                    propertySetter: propertySetterMock.Object);

                int expectedValue = 1;
                var template = new TestClassTemplate(expectedValue);
                var expectedProperties = new[]
                {
                    nameof(TestClass.PublicGetPublicSet),
                    nameof(TestClass.PublicGetInternalSet),
                    nameof(TestClass.PublicGetProtectedSet),
                    nameof(TestClass.PublicGetPrivateSet),
                    nameof(TestClass.PublicReadOnlyAutoProperty),
                    nameof(TestClass.PublicGetOfPrivateField),
                    nameof(TestClass.InternalGetInternalSet),
                    nameof(TestClass.InternalGetPrivateSet),
                    nameof(TestClass.ProtectedInternalGetProtectedInternalSet),
                    nameof(TestClass.ProtectedInternalGetInternalSet),
                    nameof(TestClass.ProtectedInternalGetPrivateSet)
                }
                .Select(propName => typeof(TestClass).GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

                // act
                sut.OverwriteWithTemplate(template);
                sut.Build();

                // assert
                foreach (var expectedProperty in expectedProperties)
                    propertySetterMock.Verify(e => e.SetProperty(It.IsAny<object>(), expectedProperty, expectedValue), Times.Once());
                propertySetterMock.Verify(e => e.SetProperty(It.IsAny<object>(), It.IsAny<PropertyInfo>(), It.IsAny<object>()), Times.Exactly(expectedProperties.Count()));
            }

            [Fact]
            public void ShouldCallRightPropertySetterForAmbiguousProperties()
            {
                // arrange
                var propertySetterMock = new Mock<IPropertySetter>();
                var sut = new DynamicBuilder<SmallTestClass>(
                    propertySetter: propertySetterMock.Object);

                int expectedValue = 1;
                var template = new SmallTestClassTemplate
                {
                    someProperty = expectedValue,
                    someproperty = expectedValue + 1
                };
                var expectedProperties = new[]
                {
                    nameof(SmallTestClass.SomeProperty)
                }
                .Select(propName => typeof(SmallTestClass).GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

                // act
                sut.OverwriteWithTemplate(template);
                sut.Build();

                // assert
                foreach (var expectedProperty in expectedProperties)
                    propertySetterMock.Verify(e => e.SetProperty(It.IsAny<object>(), expectedProperty, expectedValue), Times.Once());
                propertySetterMock.Verify(e => e.SetProperty(It.IsAny<object>(), It.IsAny<PropertyInfo>(), It.IsAny<object>()), Times.Exactly(expectedProperties.Count()));
            }

            public class TestClass
            {
                public TestClass() { }
                public TestClass(
                    int value)
                {
                    PublicGetPublicSet = value;
                    PublicGetInternalSet = value;
                    PublicGetProtectedSet = value;
                    PublicGetPrivateSet = value;
                    PublicReadOnlyAutoProperty = value;
                    _publicGetOfPrivateField = value;

                    InternalGetInternalSet = value;
                    InternalGetPrivateSet = value;

                    ProtectedInternalGetProtectedInternalSet = value;
                    ProtectedInternalGetInternalSet = value;
                    ProtectedInternalGetPrivateSet = value;

                    ProtectedGetProtectedSet = value;
                    ProtectedGetPrivateSet = value;

                    PrivateGetPrivateSet = value;
                }

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

            public class TestClassTemplate
            {
                public TestClassTemplate() { }
                public TestClassTemplate(
                    int value)
                {
                    PublicGetPublicSet = value;
                    PublicGetInternalSet = value;
                    PublicGetProtectedSet = value;
                    PublicGetPrivateSet = value;
                    PublicReadOnlyAutoProperty = value;
                    _publicGetOfPrivateField = value;

                    InternalGetInternalSet = value;
                    InternalGetPrivateSet = value;

                    ProtectedInternalGetProtectedInternalSet = value;
                    ProtectedInternalGetInternalSet = value;
                    ProtectedInternalGetPrivateSet = value;

                    ProtectedGetProtectedSet = value;
                    ProtectedGetPrivateSet = value;

                    PrivateGetPrivateSet = value;
                }

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

            class SmallTestClass
            {
                public int SomeProperty { get; set; }
            }

            class SmallTestClassTemplate
            {
                public int someProperty { get; set; }
                public int someproperty { get; set; }
            }
        }
    }
}
