using System.Collections.Generic;
using System.Linq;
using Intefaces;

namespace Extentions
{
    public static class IValueExtender
    {
        public static string ToValue(this IEnumerable<IValue> Values)
        => Values.Select(x => x.ToValue()).ToJoinedString(" ");
    }
}