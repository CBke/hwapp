using System.Collections.Generic;
using Extentions;
using System.Linq;
namespace Intefaces
{
    public interface IValue
    {
        string ToValue();
        object[] GiveMeAllYourMoney();
    }
    public static class IValueExtender
    {
        public static string ToValue(this IEnumerable<IValue> Values)
        => Values.Select(x => x.ToValue()).ToJoinedString(" ");
    }
}