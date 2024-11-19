using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WomenActivity.Models;

public class WomenActivityDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<CycleRecord> CycleRecords { get; set; }
    public DbSet<DailyWellnessRecord> DailyWellnessRecords { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Memory> Memories { get; set; }
    public DbSet<Book> BooksToRead { get; set; }
    public DbSet<Routine> Routines { get; set; }

    public WomenActivityDbContext(DbContextOptions<WomenActivityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurazione della relazione uno-a-uno tra User e UserProfile
        modelBuilder.Entity<User>()
            .HasOne(u => u.UserProfile)
            .WithOne(up => up.User)
            .HasForeignKey<UserProfile>(up => up.UserId);

        // Configurazione per Goals, Memories e Books
        modelBuilder.Entity<Goal>()
            .HasOne(g => g.UserProfile)
            .WithMany(up => up.Goals)
            .HasForeignKey(g => g.UserProfileId);

        modelBuilder.Entity<Memory>()
            .HasOne(m => m.UserProfile)
            .WithMany(up => up.Memories)
            .HasForeignKey(m => m.UserProfileId);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.UserProfile)
            .WithMany(up => up.BooksToRead)
            .HasForeignKey(b => b.UserProfileId);

        base.OnModelCreating(modelBuilder);
    }
}