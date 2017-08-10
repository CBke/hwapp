using System;
using System.IO;
using Data;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Globalization;
using System.Xml.Serialization;
using System.Reactive.Concurrency;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;

namespace hwapp
{
    class Program
    {
        static void Main(string[] args)
        {

            var before = DateTime.Now;

            Console.WriteLine($"Start");

            var number = ImportFromXml();
            var TimeSpan = ((TimeSpan)(DateTime.Now - before)).TotalMilliseconds;

            Console.WriteLine($"\rTotal : {number} in {TimeSpan} ms   => {number * 1000 / TimeSpan} / second hiha");

            //PrintItems();
        }

        public static int ImportFromXml()
        {
            var XmlSerializer = new XmlSerializer(typeof(Mods), "http://www.loc.gov/mods/v3");
            var UniqueAuthorIds = new HashSet<string>();
            var UniqueProjectIds = new HashSet<string>();
            var Projects = new List<Project>();
            var Authors = new List<Author>();
            var Publication_Author = new List<Publication_Author>();
            var Publications = new List<Publication>();

            using (var BloggingContext = new BloggingContext())
            {

                BloggingContext.Database.EnsureCreated();
                BloggingContext.Database.Migrate();
                BloggingContext.ChangeTracker.AutoDetectChangesEnabled = false;

                using (var StreamReader = new StreamReader(File.OpenRead("data.xml")))
                {
                    StreamReader
                        .XmlReaderObserver<Mods>("mods")
                        .Finally(() => BloggingContext.SaveChangesAsync())
                        .Select(x => (Mods)XmlSerializer.Deserialize(x))
                        .Subscribe(x =>
                        {
                            Publications.Add(x.MapToPublication(""));
                            Authors.AddRange(x.MapToAuthors().Where(y => UniqueAuthorIds.Add(y.Id)));
                            Publication_Author.AddRange(x.MapToPublication_Author("", x.RecordInfo.RecordIdentifier));
                            Projects.AddRange(x.MapToProject(x.RecordInfo.RecordIdentifier).Where(y => UniqueProjectIds.Add(y.Id)));
                        });

                    var t = new List<PublicatieFTS>();
                    t.AddRange(new List<PublicatieFTS>());

                    StreamReader.ReadToEnd();

                    BloggingContext.AddRangeAsync(Publications);
                    BloggingContext.SaveChangesAsync();
                    BloggingContext.AddRangeAsync(Authors);
                    BloggingContext.SaveChangesAsync();
                    BloggingContext.AddRangeAsync(Publication_Author);
                    BloggingContext.SaveChangesAsync();
                    BloggingContext.AddRangeAsync(Projects);
                    BloggingContext.SaveChangesAsync();

                }
                /*
                               MapToPublicatieFTS+

                                var Pub = x.MapToPublication(DatabaseContext, DatabaseContext.DatabaseContextSettings.PersonalPagePrefix);
                            DatabaseContext.Authors.AddRange(Pub.MapToAuthors(AuthorsUniqFilter));
                            DatabaseContext.PublicatieFTS.Add(Pub.MapToPublicatieFTS());
                            foreach (var Author in Pub.Authors)
                            {
                                Author.AuthorId = Author.Author.Id;
                                Author.Author = null;
                            }
                */

            }

            return 123212;
        }
        public static void PrintItems()
        {
            using (var BloggingContext = new BloggingContext())
            {

                var JsonSerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };

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