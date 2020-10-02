using System;
using System.Collections.Generic;
using System.Text;
using TextHighlightCore.Extensions;

namespace TextHighlightCore.Services
{
    public class ColoringRuleService : IColoringRuleService
    {
        public IDictionary<ColorRule, List<ValueTuple<int, int>>> FindRulesInText(ICollection<ColorRule> rules, string text)
        {
            IDictionary<ColorRule, List<ValueTuple<int, int>>> resultRules = new Dictionary<ColorRule, List<ValueTuple<int, int>>>();
            
            foreach(var rule in rules)
            {
                if(resultRules.ContainsKey(rule))
                {
                    continue;
                }

                if(text.Contains(rule.RuleText))
                {
                    List<ValueTuple<int, int>> listOfindexes = new List<(int, int)>();

                    int currentIndex = 0;
                    int lastIndex = text.LastIndexOf(rule.RuleText);
                    int lengthOfRule = rule.RuleText.Length;
                  
                    do
                    {
                        currentIndex = text.IndexOf(rule.RuleText, currentIndex);
                        listOfindexes.Add((currentIndex, currentIndex + lengthOfRule -1));
                        currentIndex += lengthOfRule;

                    } while (currentIndex <= lastIndex);

                    resultRules.Add(rule, listOfindexes);
                }
            }
            return resultRules;
        }
    }
}
