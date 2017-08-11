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
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Internal;

namespace hwapp
{
    public static class MainHelper
    {

        public static double SinceThen(this DateTime before) => ((TimeSpan)(DateTime.Now - before)).TotalMilliseconds;

    }

    class Program
    {



        static void Main(string[] args)
        {
            DateTime before = DateTime.Now;

            Console.WriteLine($"Start");

            var number = ImportFromXml(before);


            Console.WriteLine($"\rTotal : {number} in {before.SinceThen()} ms   => {number * 1000 / before.SinceThen()} / second hiha");

            //PrintItems();
        }

        public static int ImportFromXml(DateTime before)
        {
            var XmlSerializer = new XmlSerializer(typeof(Mods), "http://www.loc.gov/mods/v3");
            var UniqueAuthorIds = new Concurrent​Dictionary<string, string>();
            var UniqueProjectIds = new Concurrent​Dictionary<string, string>();
            var number = 0;
            var Projects = new ConcurrentQueue<Project>();
            var Authors = new ConcurrentQueue<Author>();
            var Publication_Author = new ConcurrentQueue<Publication_Author>();
            var Publications = new ConcurrentQueue<Publication>();


            using (var BloggingContext = new BloggingContext())
            {

                BloggingContext.Database.EnsureCreated();
                BloggingContext.Database.Migrate();
                BloggingContext.ChangeTracker.AutoDetectChangesEnabled = false;



                using (var StreamReader = new StreamReader(File.OpenRead("data.xml")))
                {
                    StreamReader
                        .XmlReaderObserver<Mods>("mods")
                        .Finally(() => BloggingContext.SaveChanges())
                        .Select(x => (Mods)XmlSerializer.Deserialize(x))
                        .ObserveOn(Scheduler.Default)
                        .Subscribe(x =>
                        {
                            ++number;
                            Publications.Enqueue(x.MapToPublication(""));
                            x.MapToAuthors().Where(y => UniqueAuthorIds.TryAdd(y.Id, "")).Where(y => !y.Id.Equals("/#//#//#/")).ToList().ForEach(y => Authors.Enqueue(y));
                            x.MapToPublication_Author("", x.RecordInfo.RecordIdentifier).Where(y => !y.AuthorId.Equals("/#//#//#/")).ToList().ForEach(y => Publication_Author.Enqueue(y));
                            x.MapToProject(x.RecordInfo.RecordIdentifier).Where(y => UniqueProjectIds.TryAdd(y.Id, "")).ToList().ForEach(y => Projects.Enqueue(y));
                        });



                    Console.WriteLine($"{before.SinceThen()} ms   => {number * 1000 / before.SinceThen()} / seconds Parsing");
                    StreamReader.ReadToEnd();


                    BloggingContext.AddRange(Publications);
                    BloggingContext.SaveChanges();
                    Console.WriteLine($"{before.SinceThen()} ms   => {number * 1000 / before.SinceThen()} / second Publications{Publications.Count}");
                    BloggingContext.AddRange(Authors);
                    BloggingContext.SaveChanges();
                    Console.WriteLine($"{before.SinceThen()} ms   => {number * 1000 / before.SinceThen()} / second Authors{Authors.Count}");
                    BloggingContext.AddRange(Publication_Author);
                    BloggingContext.SaveChanges();
                    Console.WriteLine($"{before.SinceThen()} ms   => {number * 1000 / before.SinceThen()} / second Publication_Author{Publication_Author.Count}");
                    BloggingContext.AddRangeAsync(Projects);
                    BloggingContext.SaveChanges();
                    Console.WriteLine($"{before.SinceThen()} ms   => {number * 1000 / before.SinceThen()} / second Projects{Projects.Count}");

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