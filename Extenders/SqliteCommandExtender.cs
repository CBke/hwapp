
using System.Collections.Generic;
using Intefaces;
using Microsoft.Data.Sqlite;

namespace Extentions
{
    public static class SqliteCommandExtender
    {
        public static void Run(this SqliteCommand SqliteCommand, IValue IValue)
        {
            int i = 0;
            foreach (var element in IValue.GiveMeAllYourMoney())
                SqliteCommand.Parameters[i++].Value = element;

            SqliteCommand.ExecuteNonQueryAsync();
        }
        public static void Run(this SqliteCommand SqliteCommand, IEnumerable<IValue> IValues)
        {
            foreach (var IValue in IValues)
                SqliteCommand.Run(IValue);
        }
    }
}