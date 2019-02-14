using System.Text.RegularExpressions;

namespace TestData.Building.Standard
{
    public static class SelectBackingField
    {
        public static ReadOnlyAutoPropertyBackingFieldSelector ForReadOnlyAutoProperty() =>
            new ReadOnlyAutoPropertyBackingFieldSelector();

        public static PropertyNameRegexReplaceBackingFieldSelector FromPropertyNameByRegex(string pattern, MatchEvaluator matchEvaluator) =>
            new PropertyNameRegexReplaceBackingFieldSelector(pattern, matchEvaluator);

        public static BackingFieldByNameSelector FromPropertyName(BackingFieldNameEvaluator evaluator) =>
            new BackingFieldByNameSelector(evaluator);
    }
}
