using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Xml.Serialization;
using Data;
using Extentions;
using Microsoft.EntityFrameworkCore;
using Models;

namespace hwapp
{
    public static class MainHelper
    {
        public static double SinceThen(this DateTime before)
        => ((TimeSpan)(DateTime.Now - before)).TotalSeconds;
    }
    class Program
    {

        static void Main(string[] args)
        {
            DateTime before = DateTime.Now;

            Console.WriteLine($"Start");

            var number = ImportFromXml("data.xml", "CalorieTracker.db");

            Console.WriteLine($"\rTotal : {number} in {before.SinceThen()} s   => {number / before.SinceThen()} / second hiha");

            PrintItems("CalorieTracker.db");
            Console.WriteLine($"{before.SinceThen()} s   => end select");

        }

        public static int ImportFromXml(string XMLFileName, string DBFileName)
        {
            var XmlSerializer = new XmlSerializer(typeof(Mods), "http://www.loc.gov/mods/v3");
            var UniqueAuthorIds = new Concurrent​Dictionary<string, string>();
            var UniqueProjectIds = new Concurrent​Dictionary<string, string>();
            var number = 0;

            using (var BloggingContext = new BloggingContext { FileName = DBFileName })
            {
                BloggingContext.Database.EnsureCreated();
                BloggingContext.Database.Migrate();
                BloggingContext.Database.OpenConnection();

                using (var SQLiteConnection = BloggingContext.Database.GetDbConnection() as Microsoft.Data.Sqlite.SqliteConnection)
                using (var Transaction = SQLiteConnection.BeginTransaction())
                {
                    var AuthorCommand = BloggingContext.ToInsertSqliteCommand(typeof(Author), SQLiteConnection, Transaction);
                    var PublicationCommand = BloggingContext.ToInsertSqliteCommand(typeof(Publication), SQLiteConnection, Transaction);
                    var Publication_AuthorCommand = BloggingContext.ToInsertSqliteCommand(typeof(Publication_Author), SQLiteConnection, Transaction);
                    var ProjectCommand = BloggingContext.ToInsertSqliteCommand(typeof(Project), SQLiteConnection, Transaction);

                    using (var StreamReader = new StreamReader(File.OpenRead(XMLFileName)))
                    {
                        StreamReader
                        .XmlReaderObserver<Mods>("mods")
                        .Select(x => (Mods)XmlSerializer.Deserialize(x))
                        .Select(x => x.ToExtractUnit(UniqueAuthorIds, UniqueProjectIds))
                        .ObserveOn(Scheduler.Default)
                        .Do(x =>
                        {
                            number++;
                            PublicationCommand.Run(x.Publication);
                            AuthorCommand.Run(x.Authors);
                            Publication_AuthorCommand.Run(x.Publication_Authors);
                            ProjectCommand.Run(x.Projects);
                        })
                        .ToTask()
                        .Wait();

                        Transaction.Commit();

                        return number;
                    }
                }
            }
        }

        public static void PrintItems(string DBFileName)
        {
            using (var BloggingContext = new BloggingContext { FileName = DBFileName })
            {
                BloggingContext.Database.OpenConnection();

                using (var SQLiteConnection = BloggingContext.Database.GetDbConnection() as Microsoft.Data.Sqlite.SqliteConnection)
                using (var Transaction = SQLiteConnection.BeginTransaction())
                {
                    var Publications = BloggingContext
                   .Publications
                   .Include(x => x.Authors)
                   .ThenInclude(x => x.Author)
                   .Include(xy => xy.Projects)
                   .Select(x => x.ToPublicatieFTS())
                   .ToList();

                    BloggingContext
                    .ToInsertSqliteCommand(typeof(PublicatieFTS), SQLiteConnection, Transaction)
                    .Run(Publications);

                    Transaction.Commit();
                }
            }
        }
    }
}