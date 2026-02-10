namespace MindfulMomentsApp.Models
{
  public class Journal
  {
    public int JournalId { get; set; }
    public string JournalName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
  }
}
