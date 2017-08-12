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
using Newtonsoft.Json;

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

            var number = ImportFromXml(before);

            Console.WriteLine($"\rTotal : {number} in {before.SinceThen()} s   => {number  / before.SinceThen()} / second hiha");

            PrintItems();
        }

        public static int ImportFromXml(DateTime before)
        {
            var XmlSerializer = new XmlSerializer(typeof(Mods), "http://www.loc.gov/mods/v3");
            var UniqueAuthorIds = new Concurrent​Dictionary<string, string>();
            var UniqueProjectIds = new Concurrent​Dictionary<string, string>();
            var number = 0;

            using (var BloggingContext = new BloggingContext())
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

                    using (var StreamReader = new StreamReader(File.OpenRead("data.xml")))
                    {
                        StreamReader
                        .XmlReaderObserver<Mods>("mods")
                        .Select(x => (Mods)XmlSerializer.Deserialize(x))
                        .ObserveOn(Scheduler.Default)
                        .Select(x => x.MapToExtractUnit(UniqueAuthorIds, UniqueProjectIds))
                        .ObserveOn(Scheduler.Default)
                        .Do(x =>
                        {
                            ++number;
                            PublicationCommand.Run(x.Publication);
                            foreach (var Author in x.Authors)
                                AuthorCommand.Run(Author);
                            foreach (var Publication_Author in x.Publication_Authors)
                                Publication_AuthorCommand.Run(Publication_Author);
                            foreach (var Project in x.Projects)
                                ProjectCommand.Run(Project);
                        })
                        .ToTask()
                        .Wait();
                        Transaction.Commit();

                        return number;
                    }
                }
            }
        }

        public static void PrintItems()
        {
            using (var BloggingContext = new BloggingContext())
            {

                var JsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };

                BloggingContext
               .Publications
               .ToList()
               .Select(x => JsonConvert.SerializeObject(x))
               .ToList()
               .ForEach(z => Console.WriteLine(z));
            }
        }
    }
}