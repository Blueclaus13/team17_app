using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MindfulMomentsApp.Data;

namespace MindfulMomentsApp.Data
{
  public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
  {
    public AppDbContext CreateDbContext(string[] args)
    {
      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

      var connectionString = "Host=dpg-d62at3ogjchc73ah6eeg-a.oregon-postgres.render.com;Port=5432;Database=team17_db;Username=team17_db_user;Password=3Yk5ND1inHVlXjJkLY5X7xTizzwIaJJu;Ssl Mode=Require;Trust Server Certificate=true";

      optionsBuilder.UseNpgsql(connectionString);
      return new AppDbContext(optionsBuilder.Options);
    }
  }
}
