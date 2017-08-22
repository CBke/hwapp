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
    class Program
    {
        static void Main(string[] args)
        {
            DateTime before = DateTime.Now;

            Console.WriteLine($"Start");

            using (var BloggingContext = new BloggingContext { FileName = "CalorieTracker.db" })
            {
                BloggingContext.Database.EnsureCreated();
                BloggingContext.Database.Migrate();
                BloggingContext.Database.OpenConnection();

                var number = BloggingContext.ImportBiBXml();

                Console.WriteLine($"\rTotal : {number} in {before.SinceThen()} s   => {number / before.SinceThen()} / second hiha");

                BloggingContext.PrintItems();
            }
            Console.WriteLine($"{before.SinceThen()} s   => end select");
        }
    }
}