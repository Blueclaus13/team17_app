using Microsoft.EntityFrameworkCore;
using MindfulMomentsApp.Models;

namespace MindfulMomentsApp.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Journal> Journals { get; set; }
    public DbSet<Entry> Entries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>(entity =>
      {
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
        entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<Entry>(entity =>
      {
        entity.HasKey(e => e.EntryId);
        entity.Property(e => e.Description).HasMaxLength(1000);
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
