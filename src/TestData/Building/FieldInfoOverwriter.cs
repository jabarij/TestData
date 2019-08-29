using System.Reflection;

namespace TestData.Building
{
    internal class FieldInfoOverwriter : NamedPropertyOverwriter, INamedPropertyOverwriter
    {
        public FieldInfoOverwriter(FieldInfo field, object originalValue = null)
            : base(Assert.IsNotNull(field, nameof(field)).Name, field.FieldType, originalValue)
        {
            Field = field;
        }

        public FieldInfo Field { get; }

        public void SetValueFromFieldOwner(object owner) =>
            SetValue(Field.GetValue(owner));
    }
}