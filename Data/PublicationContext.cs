using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Models;
using Microsoft.Data.Sqlite;
using System;

namespace Data
{
    public class PublicationContext : DbContext
    {
        public string FileName { get; set; } = "out.db";
        public DbSet<Publication> Publications { get; set; }
        public DbSet<PublicatieFTS> PublicatieFTS { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={FileName}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Project>()
                .HasKey(a => new { a.Id, a.PublicationId });

            modelBuilder
                .Entity<Publication_Author>()
                .HasKey(a => new { a.AuthorId, a.PublicationId });
        }
    }
}