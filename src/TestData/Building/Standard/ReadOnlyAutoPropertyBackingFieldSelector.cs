using System.Reflection;

namespace TestData.Building.Standard
{
    public class ReadOnlyAutoPropertyBackingFieldSelector : PropertyBackingFieldByNameSelector
    {
        protected override string GetBackingFieldName(PropertyInfo property) => $"<{property.Name}>k__BackingField";
    }
}