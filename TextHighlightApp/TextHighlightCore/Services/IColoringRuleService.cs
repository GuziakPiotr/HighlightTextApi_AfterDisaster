using System;
using System.Collections.Generic;

namespace TextHighlightCore.Services
{
    public interface IColoringRuleService
    {
        IDictionary<ColorRule, List<ValueTuple<int, int>>> FindRulesInText(ICollection<ColorRule> rules, string text);
    }
}
