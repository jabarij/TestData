using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TestData.Building.Standard
{
    public class PropertyNameRegexReplaceBackingFieldSelector : PropertyBackingFieldByNameSelector
    {
        private readonly Regex _regex;
        private readonly MatchEvaluator _matchEvaluator;

        public PropertyNameRegexReplaceBackingFieldSelector(string pattern, MatchEvaluator matchEvaluator)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled);
            _matchEvaluator = Assert.IsNotNull(matchEvaluator, nameof(matchEvaluator));
        }

        protected override string GetBackingFieldName(PropertyInfo property) =>
            _regex.Replace(property.Name, _matchEvaluator);
    }
}
