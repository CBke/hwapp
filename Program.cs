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

            using (var PublicationContext = new PublicationContext { FileName = "PublicationContext.db" })
            {
                PublicationContext.Database.EnsureCreated();
                PublicationContext.Database.Migrate();
                PublicationContext.Database.OpenConnection();

                var number = PublicationContext.ImportBiBXml();

                Console.WriteLine($"\rTotal : {number} in {before.SinceThen()} s   => {number / before.SinceThen()} / second hiha");

                PublicationContext.PrintItems();
            }
            Console.WriteLine($"{before.SinceThen()} s   => end select");
        }
    }
}