using MindfulMomentsApp.Models.Enums;

namespace MindfulMomentsApp.Models
{
    public class Entry
    {
        public int EntryId { get; set; }
        public int JournalId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public Moods Mood { get; set; }
        public Activities Activity { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
