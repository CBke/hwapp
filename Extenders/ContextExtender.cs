using System;
using System.Collections.Generic;
using System.Linq;
using Intefaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Extentions
{
    public static class ContextExtender
    {
        public static string TableName(this DbContext DbContext, Type Type)
        => DbContext.Model.FindEntityType(Type).Sqlite().TableName;

        public static IEnumerable<string> ColumnNames(this DbContext DbContext, Type Type)
        => DbContext.Model.FindEntityType(Type).GetProperties().Select(x => x.Sqlite().ColumnName);
        public static SqliteCommand ToInsertSqliteCommand(this DbContext DbContext, Type Type, SqliteConnection SQLiteConnection, SqliteTransaction SqliteTransaction)
        {
            var TableName = DbContext.TableName(Type);
            var Fields = DbContext.ColumnNames(Type).ToList();
            var FieldPlaceholder = Fields.Select(x => $":{x}").ToJoinedString();
            var SqliteCommand = new SqliteCommand($"INSERT INTO {TableName} VALUES ({FieldPlaceholder})", SQLiteConnection, SqliteTransaction);

            SqliteCommand.Parameters.AddRange(Fields.Select(x => new SqliteParameter { ParameterName = x }));

            return SqliteCommand;
        }
        public static void Run(this SqliteCommand SqliteCommand, IValue IValue)
        {
            int i = 0;
            foreach (var element in IValue.GiveMeAllYourMoney())
                SqliteCommand.Parameters[i++].Value = element;

            SqliteCommand.ExecuteNonQueryAsync();
        }
    }
}