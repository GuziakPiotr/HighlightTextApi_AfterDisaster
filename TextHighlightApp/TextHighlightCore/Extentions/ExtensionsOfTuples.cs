using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextHighlightCore.Extensions
{
    public static class ExtensionsOfTuples
    {
        public static string GetAsStringInline(this ICollection<ValueTuple<int, int>> tuples)
        {
            var resultCollection = tuples.Select(tup => $"[{tup.Item1}:{tup.Item2}]");

            return string.Join(",",resultCollection.ToArray());
        }
    }
}
