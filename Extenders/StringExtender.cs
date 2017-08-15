using System.Collections.Generic;

namespace Extentions
{
    public static class StringExtender
    {
        public static int? TryParse(this string stringDate)
        {
            int NumericValue;
            return int.TryParse(stringDate, out NumericValue) ? NumericValue : (int?)null;
        }
        public static string ToJoinedString(this IEnumerable<string> StringList, string Joiner = ",") =>
            string.Join(Joiner, StringList);
    }
}