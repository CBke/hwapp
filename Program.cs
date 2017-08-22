using System;
using Data;
using Extentions;
using Microsoft.EntityFrameworkCore;

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