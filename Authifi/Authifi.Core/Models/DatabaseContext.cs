using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;

namespace Authifi.Core.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Song> Songs { get; set; }

      

        public string DbPath { get; private set; }

       
        public DatabaseContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
           //var path = Environment.CurrentDirectory;
            DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}sqliteAuthifi.db";

            //DbPath = Path.Combine(path, "sqliteAuthifi.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Playlist>().ToTable("Playlists");
            modelBuilder.Entity<Song>().ToTable("Songs");

            /*modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Mail).IsUnique();
                entity.Property(e => e.GivenName);
                

            });

            modelBuilder.Entity<Playlist>(entity =>
            {
                entity.ToTable("Playlists");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name);
                entity.Property(e => e.Liked);
                entity.HasOne(d => d.User)
                    .WithOne()
                    .HasForeignKey<Playlist>(d => d.UserID);

            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.ToTable("Songs");
                entity.HasKey(e => e.Hash);
                entity.Property(e => e.Title);
                entity.Property(e => e.Lyrics);
                entity.HasOne(d => d.playlist)
                        .WithMany(p => p.Songs)
                        .HasForeignKey(d => d.PlaylistID);
            });*/

             
    }

       
    }
}

