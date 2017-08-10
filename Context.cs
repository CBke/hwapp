using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Models;
using Microsoft.Data.Sqlite;
using System;

namespace hwapp
{
    public class BloggingContext : DbContext
    {
     
        public DbSet<Publication> Publications { get; set; }
       // public DbSet<Author> Authors { get; set; }


          protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=CalorieTracker.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

  /*           
 modelBuilder
                .Entity<Publication_Author>()
                 .HasOne(p => p.Author)
            .WithMany(b => b.Authors)
            .HasForeignKey(p => p.AuthorId);

*/
            modelBuilder
                .Entity<Project>()
                .HasKey(a => new { a.Id, a.PublicationId })
                ;
          /*
                modelBuilder
                .Entity<Publication>()
                .HasMany(x => x.Authors);
                modelBuilder
                .Entity<Publication>()
                .HasMany(x => x.Projects);
            */    
            modelBuilder
                .Entity<Publication_Author>()
                .HasKey(a => new { a.AuthorId, a.PublicationId });
        }
    }
}