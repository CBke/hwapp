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
        public static Microsoft.Data.Sqlite.SqliteCommand ToInsertSqliteCommand(this DbContext DbContext, Type Type, Microsoft.Data.Sqlite.SqliteConnection SQLiteConnection, Microsoft.Data.Sqlite.SqliteTransaction SqliteTransaction)
        {
            var TableName = DbContext.TableName(Type);
            var Fields = DbContext.ColumnNames(Type).ToList();
            var FieldPlaceholder = Fields.Select(x => $":{x}").ToJoinedString();
            var SqliteCommand = new Microsoft.Data.Sqlite.SqliteCommand($"INSERT INTO {TableName} VALUES ({FieldPlaceholder})", SQLiteConnection, SqliteTransaction);

            SqliteCommand.Parameters.AddRange(Fields.Select(x => new Microsoft.Data.Sqlite.SqliteParameter { ParameterName = x }));

            return SqliteCommand;
        }
        public static int Run(this SqliteCommand SqliteCommand, IValue IValue)
        {
            int i = 0;
            foreach (var element in IValue.GiveMeAllYourMoney())
                SqliteCommand.Parameters[i++].Value = element;

            SqliteCommand.ExecuteNonQueryAsync();
            return 0;
        }
    }
}