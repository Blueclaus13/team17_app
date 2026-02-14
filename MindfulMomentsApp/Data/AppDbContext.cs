using Microsoft.EntityFrameworkCore;
using MindfulMomentsApp.Models;

namespace MindfulMomentsApp.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Journal> Journals { get; set; }
    public DbSet<Entry> Entries { get; set; } = default!; // added default! to suppress nullable warning

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>(entity =>
      {
        entity.HasIndex(e => e.Email).IsUnique();

       // entity.Property(e => e.PasswordHash).HasMaxLength(255); //dotnet add package BCrypt.Net-Next
        entity.HasKey(e => e.UserId);
        entity.Property(e => e.GoogleId).HasMaxLength(255);
        entity.Property(e => e.FirstName).HasMaxLength(100);
        entity.Property(e => e.LastName).HasMaxLength(100);
        entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
        entity.Property(e => e.Password).HasMaxLength(255);
        entity.Property(e => e.Photo).HasMaxLength(500);
      });

      modelBuilder.Entity<Journal>(entity =>
      {
        entity.HasKey(e => e.JournalId);
        entity.Property(e => e.JournalName).HasMaxLength(200).IsRequired();
        entity.HasOne(j => j.User)
          .WithOne(u => u.Journal)
          .HasForeignKey<Journal>(j => j.UserId)
          .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<Entry>(entity =>
      {
        entity.HasKey(e => e.EntryId);
        entity.Property(e => e.Description).HasMaxLength(1000);
        entity.Property(e => e.Mood)
          .HasConversion<string>();
        entity.Property(e => e.Activity)
          .HasConversion<string>();
          entity.HasOne(e => e.Journal)
          .WithMany(j => j.Entries)
          .HasForeignKey(e => e.JournalId)
          .OnDelete(DeleteBehavior.Cascade);

        entity.ToTable("Entries", tb => tb.HasCheckConstraint(
        "CK_UpdatedDate_SameDay",
        "\"UpdatedDate\"::date = \"CreatedDate\"::date OR \"UpdatedDate\" IS NULL"));
      });
    }
  }
}
