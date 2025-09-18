using Api_Orbis_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography.Xml;

namespace Api_Orbis_Project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }
        public DbSet<Guide> Guides { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-many relations: User -> Trips, Preferences, Alerts, ChatHistories
            modelBuilder.Entity<User>()
                .HasMany(u => u.Trips)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Preferences)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Alerts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ChatHistories)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);

            // Many-to-many: Trip <-> Guide (EF Core will create join table "TripGuide")
            modelBuilder.Entity<Trip>()
                .HasMany(t => t.Guides)
                .WithMany(g => g.Trips)
                .UsingEntity<Dictionary<string, object>>(
                    "TripGuide",
                    j => j.HasOne<Guide>().WithMany().HasForeignKey("GuideId"),
                    j => j.HasOne<Trip>().WithMany().HasForeignKey("TripId"),
                    j =>
                    {
                        j.HasKey("TripId", "GuideId");
                        j.ToTable("TripGuide");
                    });

            // Store enums as strings for readability
            modelBuilder.Entity<User>().Property(u => u.UserRole).HasConversion<string>();
            modelBuilder.Entity<Trip>().Property(t => t.Type).HasConversion<string>();
            modelBuilder.Entity<Alert>().Property(a => a.Type).HasConversion<string>();
            modelBuilder.Entity<Guide>().Property(g => g.Category).HasConversion<string>();

            // Default timestamps
            modelBuilder.Entity<ChatHistory>().Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Alert>().Property(a => a.SentAt).HasDefaultValueSql("GETDATE()");
        }
    }
}
