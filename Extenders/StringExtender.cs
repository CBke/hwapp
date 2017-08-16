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
        public static string IfNull(this string String, string NullValue = "")
        => string.IsNullOrEmpty(String) ? NullValue : String;
        public static string IfNotNull(this string String, string NotNullValue)
        => !string.IsNullOrEmpty(String) ? NotNullValue : String;
        public static string IfNotNull(this object Obj, string NotNullValue)
        => Obj != null ? NotNullValue : null;
    }
}