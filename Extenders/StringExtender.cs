using System;
using System.Collections.Generic;
using System.Globalization;
namespace Extentions
{
    public static class StringExtender
    {
        public static DateTime? TryParse(this string stringDate)
        {
            DateTime date;
            return DateTime.TryParseExact(stringDate, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ? date : (DateTime?)null;
        }
        public static string PrintableLastName(this string LastName)
        {
            if (LastName == null)
                return "";
            int charindex = LastName.IndexOf(", ", StringComparison.Ordinal);
            if (charindex > 0)
                return LastName.Substring(charindex + 2) + " " + LastName.Substring(0, charindex);
            return LastName;
        }
        public static string ToJoinedString(this IEnumerable<string> StringList, string Joiner = ",") =>
            string.Join(Joiner, StringList);
    }
}