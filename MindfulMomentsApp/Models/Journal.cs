namespace MindfulMomentsApp.Models
{
  public class Journal
  {
    public int JournalId { get; set; }
    public string JournalName { get; set; } = string.Empty;
    public List<Entry> Entries { get; set; } = new List<Entry>();
    public Guid UserId { get; set; }
  }
}
