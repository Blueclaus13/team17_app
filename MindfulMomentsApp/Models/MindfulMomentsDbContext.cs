using Microsoft.EntityFrameworkCore;

namespace MindfulMomentsApp.Models
{
    public class MindfulMomentsDbContext : DbContext
    {
        public MindfulMomentsDbContext(DbContextOptions<MindfulMomentsDbContext> options)
            : base(options)
        {
        }

        // This tells the app that there is a table called "Entries" in the database
        // and it should follow the structure of your "Entry.cs" model.
        public DbSet<Entry> Entries { get; set; }
    }
}