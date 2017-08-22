using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Xml.Serialization;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Extentions
{
    public static class BloggingContextExtender
    {
        public static int ImportBiBXml(this DbContext DbContext)
        {
            var XmlSerializer = new XmlSerializer(typeof(Mods), "http://www.loc.gov/mods/v3");
            var UniqueAuthorIds = new Concurrent​Dictionary<string, string>();
            var UniqueProjectIds = new Concurrent​Dictionary<string, string>();
            var number = 0;

            using (var SQLiteConnection = DbContext.Database.GetDbConnection() as Microsoft.Data.Sqlite.SqliteConnection)
            using (var Transaction = SQLiteConnection.BeginTransaction())
            using (var AuthorCommand = DbContext.ToInsertSqliteCommand(typeof(Author), SQLiteConnection, Transaction))
            using (var PublicationCommand = DbContext.ToInsertSqliteCommand(typeof(Publication), SQLiteConnection, Transaction))
            using (var Publication_AuthorCommand = DbContext.ToInsertSqliteCommand(typeof(Publication_Author), SQLiteConnection, Transaction))
            using (var ProjectCommand = DbContext.ToInsertSqliteCommand(typeof(Project), SQLiteConnection, Transaction))
            using (var StreamReader = new StreamReader(File.OpenRead("data.xml")))
            {
                StreamReader
                .XmlReaderObserver<Mods>("mods")
                .Select(x => (Mods)XmlSerializer.Deserialize(x))
                .Select(x => x.ToExtractUnit(UniqueAuthorIds, UniqueProjectIds))
                .ObserveOn(Scheduler.Default)
                .Do(x =>
                {
                    PublicationCommand.Run(x.Publication);
                    AuthorCommand.Run(x.Authors);
                    Publication_AuthorCommand.Run(x.Publication_Authors);
                    ProjectCommand.Run(x.Projects);
                    number++;
                })
                .ToTask()
                .Wait();

                Transaction.Commit();

                return number;
            }
        }
        public static void PrintItems(this BloggingContext BloggingContext)
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