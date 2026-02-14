using System.ComponentModel.DataAnnotations.Schema;
namespace MindfulMomentsApp.Models
{
  public class Journal
  {
    public int JournalId { get; set; }
    public string JournalName { get; set; } = string.Empty;
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
  }
}
