﻿using System;
using System.Reflection;

namespace TestData.Building.Standard
{
    public class BackingFieldByNameSelector : PropertyBackingFieldByNameSelector
    {
        private readonly BackingFieldNameEvaluator _getBackingFieldName;

        public BackingFieldByNameSelector(BackingFieldNameEvaluator getBackingFieldName)
        {
            _getBackingFieldName = Assert.IsNotNull(getBackingFieldName, nameof(getBackingFieldName));
        }

        protected override string GetBackingFieldName(PropertyInfo property) => _getBackingFieldName(property);
    }
}
